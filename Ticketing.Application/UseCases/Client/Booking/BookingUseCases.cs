using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Ticketing.Application.Model.DTOs;
using Ticketing.Application.UseCases.Client.Booking.Interfaces;
using Ticketing.Application.UseCases.Client.Event.Interfaces;
using Ticketing.Domain.Domain.Event.Interfaces;
using Ticketing.Infrastructure.DTOs.Admin.Event.Request;
using Ticketing.Infrastructure.DTOs.Admin.EventSeatInventory.Response;
using Ticketing.Infrastructure.DTOs.Client.Booking.Request;
using Ticketing.Infrastructure.DTOs.Client.Booking.Response;
using Ticketing.Infrastructure.DTOs.Client.Event.Response;
using Ticketing.Infrastructure.DTOs.Client.Idempotency.Request;
using Ticketing.Infrastructure.DTOs.Client.Idempotency.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Repositories.Event;
using Ticketing.Infrastructure.Repositories.Idempotency;
using Ticketing.Infrastructure.Repositories.SeatHold;

namespace Ticketing.Application.UseCases.Client.Booking;

/// <summary>
/// Client-side Event Use Cases Implementation
/// Orchestrate giữa Controller và Domain Service
/// Tích hợp Idempotency Pattern để chống Double-click
/// </summary>
public class BookingUseCases(
    ISeatHoldUnitOfWork seatHoldUnitOfWork,
    IEventDomainService eventDomain,
    IEventUnitOfWork eventUnitOfWork,
    IIdempotencyRequestRepository idempotencyRepository) : IBookingUseCases
{
    public async Task<ResponseMessage<BookingHoldSeatDto>> HoldSeat(BookingHoldSeatRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            //Bước 1: Kiểm tra Idempotency (Chống Double-click)
            var idempotencyKey = $"hold-evt{request.event_id}_{request.idempotency_key}";
            var requestHash = ComputeHash(request);

            // Kiểm tra xem yêu cầu này đã được xử lý rồi không
            var existingRequest = await idempotencyRepository
                .GetByKeyAsync<IdempotencyRequestResponseDto>(new { idempotency_key = idempotencyKey }, cancellationToken);

            // Nếu đã xử lý xong (completed), trả về kết quả cũ từ cache
            //Trạng thái xử lý idempotency: processing, completed, failed, expired.
            if (existingRequest != null && existingRequest.status == "completed" && !string.IsNullOrEmpty(existingRequest.response_snapshot))
            {
                var cachedResponse = JsonSerializer.Deserialize<BookingHoldSeatDto>(existingRequest.response_snapshot);
                return new ResponseMessage<BookingHoldSeatDto>().MessageSuccess(cachedResponse, "Request already processed - cached result");
            }

            // Nếu đang xử lý (processing), từ chối xử lý lại để chống race condition
            if (existingRequest != null && existingRequest.status == "processing")
            {
                return new ResponseMessage<BookingHoldSeatDto>().MessageError("Yêu cầu đang được xử lý, vui lòng chờ...");
            }

            // Bước 2: Tạo mới bản ghi idempotency với status = processing
            var idempotencyInsertDto = new IdempotencyRequestInsertDto
            {
                idempotency_key = idempotencyKey,
                request_type = "hold_seats",
                user_id = request.user_id,
                request_hash = requestHash,
                status = "processing",
                response_snapshot = JsonSerializer.Serialize(new BookingHoldSeatDto()),
                expired_at = DateTime.UtcNow.AddMinutes(10)
            };

            var idempotencyId = await idempotencyRepository.InsertAsync(idempotencyInsertDto, cancellationToken);

            if (string.IsNullOrEmpty(idempotencyId))
            {
                return new ResponseMessage<BookingHoldSeatDto>().MessageError("Không thể tạo bản ghi idempotency, vui lòng thử lại");
            }

            // Bước 3: Thiết lập khóa phân tán (Redis Seat-level Lock)
            // Chống overselling bằng Redis lock trước khi vào Database
            // Implementation: Sử dụng Redis hoặc DistributedLock service
            // ...

            // Bước 4: Xác thực Dữ liệu và Optimistic Locking tại Database
            // Backend truy vấn event_seat_inventory để lấy thông tin các ghế
            // Kiểm tra seat_status = 'available'
            // Lấy version hiện tại cho optimistic locking
            // ...

            // Bước 5: Mở Transaction và Lưu dữ liệu (Database Transaction)
            // Tạo phiên giữ chỗ: Insert vào seat_hold
            // Lưu chi tiết ghế: Insert vào seat_hold_item
            // Cập nhật kho vé: Update event_seat_inventory
            // Commit Transaction
            var processResult = await ProcessHoldSeat(request, cancellationToken);
            if (processResult.issuccess)
            {
                // Bước 6: Kích hoạt thông báo Realtime (SignalR)
                // Giải phóng Redis locks
                // Bắn SignalR notification đến tất cả client xem sự kiện này
                // ...

                // Bước 7: Lập lịch Hủy tự động (Job Expire Hold)
                // Phải lên lịch Job chạy sau 10 phút để auto-expire hold nếu chưa thanh toán
                // Sử dụng Hangfire, Quartz.NET hoặc Redis Key Expiration
                // ...

                // Bước 8: Cập nhật idempotency record với status = completed và lưu response snapshot
                var idempotencyUpdateDto = new IdempotencyRequestUpdateDto
                {
                    idempotency_id = long.Parse(idempotencyId),
                    idempotency_key = idempotencyKey,
                    request_type = "hold_seats",
                    user_id = request.user_id,
                    request_hash = requestHash,
                    status = "completed",
                    response_snapshot = JsonSerializer.Serialize(processResult.data),
                    expired_at = DateTime.UtcNow.AddMinutes(10)
                };

                await idempotencyRepository.UpdateAsync(idempotencyUpdateDto, cancellationToken);

                // Bước 9: Trả kết quả cho Web Client
                // Frontend nhận hold_id và hold_expires_at
                // Frontend bắt đầu chạy countdown timer
                return processResult;
            }

            return new ResponseMessage<BookingHoldSeatDto>().MessageError(processResult.message ?? "Giữ chỗ thất bại, vui lòng thử lại");
        }
        catch (Exception e)
        {
            return new ResponseMessage<BookingHoldSeatDto>().MessageError(e.Message ?? "Giữ chỗ thất bại, vui lòng thử lại");
        }
    }

    private async Task<ResponseMessage<BookingHoldSeatDto>> ProcessHoldSeat(BookingHoldSeatRequest request, CancellationToken cancellationToken)
    {
        var result = new ResponseMessage<BookingHoldSeatDto>();
        // Backend truy vấn event_seat_inventory để lấy thông tin các ghế
        // Kiểm tra seat_status = 'available'
        // Lấy version hiện tại cho optimistic locking
        var eventDetail = await eventDomain.GetEventDetail(new EventGetByIdRequest { event_id = request.event_id }, cancellationToken);

        if (!eventDetail.issuccess)
            return new ResponseMessage<BookingHoldSeatDto>().MessageError("Không thể lấy thông tin sự kiện, vui lòng thử lại");

        var eventDto = eventDetail.data;

        //Kiểm tra tất cả các ghế khách hàng chọn phải còn trống (available)
        //Lấy tất cả thông tin ghế request.seat_ids từ eventDto để kiểm tra
        var allSeats = eventDto?.zones.SelectMany(z => z.seats).ToList() ?? new List<EventVenueSeatDto>();

        if (allSeats.Count == 0)
            return new ResponseMessage<BookingHoldSeatDto>().MessageError("Sự kiện không có ghế nào, vui lòng thử lại");

        //Nếu ghế khách chọn không có trong zones/seats của event, trả về lỗi
        var seatStatusList = allSeats.Where(s => request.seat_ids.Contains(s.seat_id)).ToList();
        if (seatStatusList.Count != request.seat_ids.Count)
            return new ResponseMessage<BookingHoldSeatDto>().MessageError("Có ghế không tồn tại trong sự kiện, vui lòng thử lại");

        //Kiểm tra xem có ghế nào không còn trống (available) không, nếu có trả về lỗi
        var unavailableSeats = seatStatusList.Where(s => s.status != "available").ToList();
        if (unavailableSeats.Count > 0)
        {
            var unavailableSeatIds = string.Join(", ", unavailableSeats.Select(s => s.seat_label));
            return new ResponseMessage<BookingHoldSeatDto>().MessageError($"Ghế {unavailableSeatIds} bạn vừa chọn, không còn trống. Vui lòng chọn ghế khác");
        }


        // Tạo phiên giữ chỗ: Insert vào seat_hold
        // Lưu chi tiết ghế: Insert vào seat_hold_item
        // Cập nhật kho vé: Update event_seat_inventory
        // Commit Transaction
        try
        {
            await seatHoldUnitOfWork.OpenAsync(cancellationToken);

            var holdStartedAt = DateTime.UtcNow;
            var holdExpiresAt = DateTime.UtcNow.AddMinutes(10);
            var holdCode = $"HOLD-{Guid.NewGuid()}";

            // Insert vào seat_hold và seat_hold_item
            var holdId = await seatHoldUnitOfWork.SeatHoldRepository.InsertAsync(new
            {
                hold_code = holdCode,
                event_id = request.event_id,
                user_id = request.user_id,
                status = "active",
                hold_started_at = holdStartedAt,
                hold_expires_at = holdExpiresAt,
            }, cancellationToken)!.ToIntAsync();

            if (holdId <= 0)
                throw new Exception("Không thể tạo phiên giữ chỗ, vui lòng thử lại");

            if (holdId > 0)
            {
                result.data = new BookingHoldSeatDto
                {
                    hold_id = holdId,
                    hold_expires_at = holdExpiresAt,
                    event_id = request.event_id,
                    hold_started_at = holdStartedAt,
                    held_seats = new List<HeldSeatItemDto>()
                };
                foreach (var seatInZone in seatStatusList)
                {
                    var zone = eventDto?.zones.FirstOrDefault(z => z.seats.Any(s => s.seat_id == seatInZone.seat_id));

                    if (zone != null)
                    {
                        seatInZone.price = zone.current_price; // Lấy giá hiện tại từ zone  
                        int seatInventoryId = seatInZone.event_seat_inventory_id > 0 ? (int)seatInZone.event_seat_inventory_id : 0;

                        if (seatInventoryId == 0)
                            throw new Exception($"Ghế {seatInZone.seat_number} không có trong kho vé, vui lòng thử lại");

                        // Lưu chi tiết ghế: Insert vào seat_hold_item
                        var holdItemId = await seatHoldUnitOfWork.SeatHoldItemRepository.InsertAsync(new
                        {
                            hold_id = holdId,
                            event_seat_inventory_id = seatInventoryId,
                            seat_id = seatInZone.seat_id,
                            zone_id = zone.event_zone_id,
                            price_at_hold = seatInZone.price,
                            seat_label_snapshot = seatInZone.seat_label,
                            zone_name_snapshot = zone.zone_name,
                            // COMMENT ON COLUMN ticketing.seat_hold_item.status IS 'Trạng thái dòng hold item: active, released, converted, expired.';
                            status = "active"
                        }, cancellationToken)!.ToIntAsync();

                        if (holdItemId <= 0)
                            throw new Exception($"Không thể tạo chi tiết giữ chỗ cho ghế {seatInZone.seat_number}, vui lòng thử lại");

                        //Cập nhật : event_seat_inventory 
                        var updateInventory = await seatHoldUnitOfWork.EventSeatInventoryRepository.UpdateHoldAsync(new
                        {
                            event_seat_inventory_id = seatInventoryId,
                            event_id = request.event_id,
                            seat_id = seatInZone.seat_id,
                            event_zone_id = zone.event_zone_id,
                            seat_status = "held",
                            current_hold_id = holdId,
                            base_price = seatInZone.price,
                            version = 1
                        }, cancellationToken)!.ToBoolAsync();
                        if (!updateInventory)
                        {
                            throw new Exception($"Không thể cập nhật kho vé cho ghế {seatInZone.seat_number}, vui lòng thử lại");
                        }

                        var holdItem = new HeldSeatItemDto
                        {
                            event_seat_inventory_id = seatInventoryId,
                            seat_hold_item_id = holdItemId ?? 0
                        };
                        result.data.held_seats.Add(holdItem);
                    }
                }
            }

            await seatHoldUnitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return new ResponseMessage<BookingHoldSeatDto>().MessageSuccess(result.data, "Giữ chỗ thành công");
        }
        catch (Exception e)
        {
            await seatHoldUnitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<BookingHoldSeatDto>().MessageError($"Lỗi khi giữ chỗ: {e.Message}");
        }
    }

    /// <summary>
    /// Helper method: Compute SHA256 hash của request payload
    /// Dùng để detect nếu client gửi payload khác nhau
    /// </summary>
    private string ComputeHash(BookingHoldSeatRequest request)
    {
        try
        {
            var json = JsonSerializer.Serialize(request);
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
            return Convert.ToBase64String(hashedBytes);
        }
        catch
        {
            return string.Empty;
        }
    }
}