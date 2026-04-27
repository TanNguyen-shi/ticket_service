using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Ticketing.Application.Model.DTOs;
using Ticketing.Application.UseCases.Client.Booking.Interfaces;
using Ticketing.Infrastructure.DTOs.Admin.EventSeatInventory.Response;
using Ticketing.Infrastructure.DTOs.Client.Booking.Request;
using Ticketing.Infrastructure.DTOs.Client.Booking.Response;
using Ticketing.Infrastructure.DTOs.Client.Idempotency.Request;
using Ticketing.Infrastructure.DTOs.Client.Idempotency.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Repositories.Idempotency;
using Ticketing.Infrastructure.Repositories.SeatHold;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Ticketing.Application.UseCases.Client.Booking;

public class BookingUseCases(
    ISeatHoldUnitOfWork seatHoldUnitOfWork,
    IIdempotencyRequestRepository idempotencyRepository) : IBookingUseCases
{
    public async Task<ResponseMessage<BookingHoldSeatDto>> HoldSeat(BookingHoldSeatRequest request, long customerId, CancellationToken cancellationToken = default)
    {
        if (customerId <= 0)
            return new ResponseMessage<BookingHoldSeatDto>().MessageError("Vui lòng đăng nhập để tiếp tục");

        var idempotencyKey = $"hold-evt{request.event_id}_{request.idempotency_key}";
        var requestHash = ComputeHash(request);
        long idempotencyIdLong = 0;

        try
        {
            var existingRequest = await idempotencyRepository
                .GetByKeyAsync<IdempotencyRequestResponseDto>(new { idempotency_key = idempotencyKey }, cancellationToken);

            if (existingRequest?.status == "completed" && !string.IsNullOrEmpty(existingRequest.response_snapshot))
            {
                var cachedResponse = JsonSerializer.Deserialize<BookingHoldSeatDto>(existingRequest.response_snapshot);
                if (cachedResponse is not null)
                    return new ResponseMessage<BookingHoldSeatDto>().MessageSuccess(cachedResponse, "Request already processed - cached result");
            }

            if (existingRequest?.status == "processing")
                return new ResponseMessage<BookingHoldSeatDto>().MessageError("Yêu cầu đang được xử lý, vui lòng chờ...");

            var idempotencyId = await idempotencyRepository.InsertAsync(new
            {
                idempotency_key = idempotencyKey,
                request_type = "hold_seats",
                customer_id = customerId,
                request_hash = requestHash,
                status = "processing",
                response_snapshot = JsonConvert.SerializeObject(new BookingHoldSeatDto()),
                expired_at = DateTime.UtcNow.AddMinutes(10)
            }, cancellationToken);

            if (string.IsNullOrEmpty(idempotencyId) || !long.TryParse(idempotencyId, out idempotencyIdLong))
                return new ResponseMessage<BookingHoldSeatDto>().MessageError("Không thể tạo bản ghi idempotency, vui lòng thử lại");

            // TODO: Bước 3 - Acquire Redis distributed locks per seat (lock:event:{eventId}:seat:{seatId})

            var processResult = await ProcessHoldSeat(request, customerId, cancellationToken);

            // Update idempotency to completed or failed regardless of outcome
            await idempotencyRepository.UpdateAsync(new
            {
                idempotency_id = idempotencyIdLong,
                idempotency_key = idempotencyKey,
                request_type = "hold_seats",
                customer_id = customerId,
                request_hash = requestHash,
                status = processResult.issuccess ? "completed" : "failed",
                response_snapshot = processResult.issuccess ? JsonSerializer.Serialize(processResult.data) : null,
                expired_at = DateTime.UtcNow.AddMinutes(10)
            }, cancellationToken);

            // TODO: Bước 6 - SignalR notification (if success)
            // TODO: Bước 7 - Schedule auto-expire job (if success)

            return processResult.issuccess
                ? processResult
                : new ResponseMessage<BookingHoldSeatDto>().MessageError(processResult.message ?? "Giữ chỗ thất bại, vui lòng thử lại");
        }
        catch (Exception e)
        {
            if (idempotencyIdLong > 0)
            {
                try
                {
                    await idempotencyRepository.UpdateAsync(new 
                    {
                        idempotency_id = idempotencyIdLong,
                        idempotency_key = idempotencyKey,
                        request_type = "hold_seats",
                        customer_id = customerId,
                        request_hash = requestHash,
                        status = "failed",
                        response_snapshot = JsonConvert.SerializeObject(new BookingHoldSeatDto()),
                        expired_at = DateTime.UtcNow.AddMinutes(10)
                    }, cancellationToken);
                }
                catch
                {
                    /* best effort */
                }
            }

            return new ResponseMessage<BookingHoldSeatDto>().MessageError(e.Message);
        }
    }

    private async Task<ResponseMessage<BookingHoldSeatDto>> ProcessHoldSeat(
        BookingHoldSeatRequest request, long userId, CancellationToken cancellationToken)
    {
        try
        {
            await seatHoldUnitOfWork.OpenAsync(cancellationToken);

            // Query real-time inventory directly from DB (not through cache) to get current status + version
            var seatInventories = (await seatHoldUnitOfWork.EventSeatInventoryRepository
                .GetBySeatIds<EventSeatInventoryDto>(new
                {
                    event_id = request.event_id,
                    seat_ids = request.seat_ids.ToArray()
                }, cancellationToken)).ToList();

            if (seatInventories.Count == 0)
                return new ResponseMessage<BookingHoldSeatDto>().MessageError("Sự kiện không có thông tin ghế, vui lòng thử lại");

            if (seatInventories.Count != request.seat_ids.Count)
                return new ResponseMessage<BookingHoldSeatDto>().MessageError("Có ghế không tồn tại trong sự kiện, vui lòng thử lại");

            var unavailableSeats = seatInventories.Where(s => s.seat_status != "available").ToList();
            if (unavailableSeats.Count > 0)
            {
                var labels = string.Join(", ", unavailableSeats.Select(s => s.seat_label));
                return new ResponseMessage<BookingHoldSeatDto>().MessageError($"Ghế {labels} không còn trống, vui lòng chọn ghế khác");
            }

            await seatHoldUnitOfWork.BeginTransactionAsync(cancellationToken);

            var holdStartedAt = DateTime.UtcNow;
            var holdExpiresAt = holdStartedAt.AddMinutes(10);

            var holdId = await seatHoldUnitOfWork.SeatHoldRepository.InsertAsync(new
            {
                hold_code = $"HOLD-{Guid.NewGuid()}",
                event_id = request.event_id,
                customer_id = userId,
                status = "active",
                hold_started_at = holdStartedAt,
                hold_expires_at = holdExpiresAt,
            }, cancellationToken)!.ToIntAsync();

            if (holdId <= 0)
                throw new Exception("Không thể tạo phiên giữ chỗ, vui lòng thử lại");

            var heldSeats = new List<HeldSeatItemDto>();

            foreach (var inventory in seatInventories)
            {
                var holdItemId = await seatHoldUnitOfWork.SeatHoldItemRepository.InsertAsync(new
                {
                    hold_id = holdId,
                    event_seat_inventory_id = inventory.event_seat_inventory_id,
                    seat_id = inventory.seat_id,
                    zone_id = inventory.event_zone_id,
                    price_at_hold = inventory.base_price,
                    seat_label_snapshot = inventory.seat_label,
                    zone_name_snapshot = inventory.zone_name,
                    status = "active"
                }, cancellationToken)!.ToIntAsync();

                if (holdItemId <= 0)
                    throw new Exception($"Không thể tạo chi tiết giữ chỗ cho ghế {inventory.seat_label}, vui lòng thử lại");

                // Pass actual version from DB for optimistic locking
                var updateSuccess = await seatHoldUnitOfWork.EventSeatInventoryRepository.UpdateHoldAsync(new
                {
                    event_seat_inventory_id = inventory.event_seat_inventory_id,
                    event_id = request.event_id,
                    seat_id = inventory.seat_id,
                    event_zone_id = inventory.event_zone_id,
                    seat_status = "held",
                    current_hold_id = holdId,
                    base_price = inventory.base_price,
                    version = inventory.version
                }, cancellationToken)!.ToBoolAsync();

                if (!updateSuccess)
                    throw new Exception($"Ghế {inventory.seat_label} vừa được người khác đặt, vui lòng thử lại");

                heldSeats.Add(new HeldSeatItemDto
                {
                    event_seat_inventory_id = inventory.event_seat_inventory_id,
                    seat_hold_item_id = holdItemId ?? 0
                });
            }

            await seatHoldUnitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return new ResponseMessage<BookingHoldSeatDto>().MessageSuccess(new BookingHoldSeatDto
            {
                hold_id = holdId,
                hold_expires_at = holdExpiresAt,
                event_id = request.event_id,
                hold_started_at = holdStartedAt,
                held_seats = heldSeats
            }, "Giữ chỗ thành công");
        }
        catch (Exception e)
        {
            await seatHoldUnitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<BookingHoldSeatDto>().MessageError($"Lỗi khi giữ chỗ: {e.Message}");
        }
    }

    private static string ComputeHash(BookingHoldSeatRequest request)
    {
        try
        {
            var json = JsonSerializer.Serialize(request);
            var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(json));
            return Convert.ToBase64String(hashedBytes);
        }
        catch
        {
            return string.Empty;
        }
    }
}