using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Ticketing.Application.Model.DTOs;
using Ticketing.Application.UseCases.Client.Booking.Interfaces;
using Ticketing.Infrastructure.DTOs.Admin.EventSeatInventory.Response;
using Ticketing.Infrastructure.DTOs.Client.Booking.Request;
using Ticketing.Infrastructure.DTOs.Client.Booking.Response;
using Ticketing.Infrastructure.DTOs.Client.Idempotency.Request;
using Ticketing.Infrastructure.DTOs.Client.Idempotency.Response;
using Ticketing.Infrastructure.DTOs.Client.SeatHold.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Repositories.Idempotency;
using Ticketing.Infrastructure.Repositories.Payment;
using Ticketing.Infrastructure.Repositories.SeatHold;
using Ticketing.Infrastructure.Repositories.Ticket;
using Ticketing.Infrastructure.Repositories.TicketOrder;
using Ticketing.Infrastructure.Repositories.TicketOrderItem;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Ticketing.Application.UseCases.Client.Booking;

public class BookingUseCases(
    ISeatHoldUnitOfWork seatHoldUnitOfWork,
    IIdempotencyRequestRepository idempotencyRepository,
    ITicketOrderRepository ticketOrderRepository,
    ITicketOrderItemRepository ticketOrderItemRepository,
    ITicketRepository ticketRepository,
    IPaymentTransactionRepository paymentTransactionRepository) : IBookingUseCases
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

            var processResult = await ProcessHoldSeat(request, customerId, cancellationToken);

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

    public async Task<ResponseMessage<CheckoutResponseDto>> CheckoutAsync(
        CheckoutRequest request,
        long customerId,
        CancellationToken cancellationToken = default)
    {
        // Bước 1: Kiểm tra đầu vào cơ bản trước khi mở kết nối DB
        if (customerId <= 0)
            return new ResponseMessage<CheckoutResponseDto>().MessageError("Vui lòng đăng nhập để tiếp tục");

        if (request.hold_id <= 0)
            return new ResponseMessage<CheckoutResponseDto>().MessageError("Thông tin giữ chỗ không hợp lệ");

        try
        {
            // Bước 2: Mở kết nối DB (chưa bắt đầu transaction)
            await seatHoldUnitOfWork.OpenAsync(cancellationToken);

            // Bước 3: Đọc thông tin phiên giữ chỗ từ DB
            var hold = await seatHoldUnitOfWork.SeatHoldRepository
                .GetAsync<SeatHoldForCheckoutDto>(new { hold_id = request.hold_id }, cancellationToken);

            // Bước 4: Kiểm tra tính hợp lệ của phiên giữ chỗ
            if (hold is null)
                return new ResponseMessage<CheckoutResponseDto>().MessageError("Không tìm thấy thông tin giữ chỗ");

            // Đảm bảo khách hàng chỉ thanh toán phiên do chính mình tạo
            if (hold.customer_id != customerId)
                return new ResponseMessage<CheckoutResponseDto>().MessageError("Bạn không có quyền thanh toán phiên giữ chỗ này");

            // Chỉ chấp nhận phiên đang active (chưa hết hạn, chưa converted, chưa released)
            if (hold.status != "active")
                return new ResponseMessage<CheckoutResponseDto>().MessageError("Phiên giữ chỗ không còn hiệu lực");

            if (hold.hold_expires_at <= DateTime.UtcNow)
                return new ResponseMessage<CheckoutResponseDto>().MessageError("Phiên giữ chỗ đã hết hạn, vui lòng thực hiện lại");

            // Bước 5: Đọc danh sách ghế trong phiên giữ chỗ
            var holdItems = (await seatHoldUnitOfWork.SeatHoldItemRepository
                .GetByHoldIdAsync<SeatHoldItemForCheckoutDto>(new { hold_id = request.hold_id }, cancellationToken)).ToList();

            if (holdItems.Count == 0)
                return new ResponseMessage<CheckoutResponseDto>().MessageError("Phiên giữ chỗ không có thông tin ghế");

            // Bước 6: Bắt đầu transaction — tất cả các bước dưới phải thành công hoặc rollback toàn bộ
            await seatHoldUnitOfWork.BeginTransactionAsync(cancellationToken);

            var now = DateTime.UtcNow;
            var totalAmount = holdItems.Sum(i => i.price_at_hold);
            var orderCode = $"ORDER-{Guid.NewGuid():N}".ToUpper();

            // Bước 7: Tạo đơn hàng (ticket_order) với trạng thái paid ngay vì mock payment luôn thành công
            var orderId = await ticketOrderRepository.InsertAsync(new
            {
                order_code = orderCode,
                event_id = hold.event_id,
                customer_id = customerId,
                hold_id = hold.hold_id,
                total_amount = totalAmount,
                discount_amount = 0m,
                final_amount = totalAmount,
                order_status = "paid",
                expired_at = DateTime.Now.AddDays(30),
                paid_at = now
            }, cancellationToken)!.ToLongAsync();

            if (orderId is null or <= 0)
                throw new Exception("Không thể tạo đơn hàng, vui lòng thử lại");

            var ticketItems = new List<CheckoutTicketItemDto>();

            // Bước 8: Xử lý từng ghế trong phiên giữ chỗ
            foreach (var item in holdItems)
            {
                // 8a: Tạo chi tiết đơn hàng (ticket_order_item) cho từng ghế
                var orderItemId = await ticketOrderItemRepository.InsertAsync(new
                {
                    order_id = orderId,
                    event_seat_inventory_id = item.event_seat_inventory_id,
                    seat_id = item.seat_id,
                    zone_id = item.zone_id,
                    unit_price = item.price_at_hold,
                    seat_label_snapshot = item.seat_label_snapshot,
                    zone_name_snapshot = item.zone_name_snapshot,
                    item_status = "paid"
                }, cancellationToken)!.ToLongAsync();

                if (orderItemId is null or <= 0)
                    throw new Exception($"Không thể tạo chi tiết đơn hàng cho ghế {item.seat_label_snapshot}");

                // 8b: Cập nhật trạng thái inventory ghế từ "held" → "sold"
                // version=1 là placeholder vì stored proc chỉ SET version chứ không dùng để WHERE (không phải true optimistic lock)
                await seatHoldUnitOfWork.EventSeatInventoryRepository.UpdateOrderAsync(new
                {
                    event_seat_inventory_id = item.event_seat_inventory_id,
                    event_id = item.event_id,
                    seat_id = item.seat_id,
                    event_zone_id = item.zone_id,
                    seat_status = "sold",
                    current_order_item_id = orderItemId,
                    base_price = item.price_at_hold,
                    version = 1
                }, cancellationToken);

                // 8c: Phát hành vé (ticket) cho ghế này
                var ticketCode = $"TKT-{Guid.NewGuid():N}".ToUpper();

                var ticketId = await ticketRepository.InsertAsync(new
                {
                    ticket_code = ticketCode,
                    order_item_id = orderItemId,
                    event_id = item.event_id,
                    customer_id = customerId,
                    seat_id = item.seat_id,
                    seat_label_snapshot = item.seat_label_snapshot,
                    zone_name_snapshot = item.zone_name_snapshot,
                    event_name_snapshot = item.event_name ?? hold.event_name ?? string.Empty,
                    ticket_status = "active",
                    issued_at = now,
                }, cancellationToken)!.ToLongAsync();

                if (ticketId is null or <= 0)
                    throw new Exception($"Không thể tạo vé cho ghế {item.seat_label_snapshot}");

                ticketItems.Add(new CheckoutTicketItemDto
                {
                    ticket_id = ticketId.Value,
                    ticket_code = ticketCode,
                    seat_label = item.seat_label_snapshot,
                    zone_name = item.zone_name_snapshot,
                    price = item.price_at_hold
                });
            }

            // Bước 9: Đánh dấu phiên giữ chỗ đã được chuyển đổi thành đơn hàng
            await seatHoldUnitOfWork.SeatHoldRepository.UpdateAsync(new
            {
                hold_id = hold.hold_id,
                hold_code = hold.hold_code,
                event_id = hold.event_id,
                customer_id = hold.customer_id,
                status = "converted",
                hold_started_at = hold.hold_started_at,
                hold_expires_at = hold.hold_expires_at,
                released_at = DateTime.UtcNow,
                release_reason = "Đã tiến hành thanh toán và chuyển đổi thành đơn hàng"
            }, cancellationToken);

            // Bước 10: Bulk-update toàn bộ seat_hold_item của phiên này sang "converted"
            await seatHoldUnitOfWork.SeatHoldItemRepository.UpdateStatusByHoldIdAsync(new
            {
                hold_id = hold.hold_id,
                status = "converted"
            }, cancellationToken);

            // Bước 11: Ghi nhận giao dịch thanh toán mock (luôn thành công, không gọi payment gateway)
            await paymentTransactionRepository.InsertAsync(new
            {
                order_id = orderId,
                payment_provider = "mock",
                payment_ref = $"MOCK-{Guid.NewGuid():N}".ToUpper(),
                provider_transaction_ref = $"MOCK-{Guid.NewGuid():N}".ToUpper(),
                amount = totalAmount,
                payment_status = "success",
                requested_at = now,
                confirmed_at = now,
                raw_request_payload = JsonConvert.SerializeObject(new
                {
                    order_id = orderId,
                    amount = totalAmount,
                    payment_method = "mock"
                }),
                raw_callback_payload = JsonConvert.SerializeObject(new
                {
                    transaction_status = "success",
                    payment_ref = $"MOCK-{Guid.NewGuid():N}".ToUpper(),
                    provider_transaction_ref = $"MOCK-{Guid.NewGuid():N}".ToUpper(),
                })
            }, cancellationToken);

            // Bước 12: Commit toàn bộ transaction
            await seatHoldUnitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return new ResponseMessage<CheckoutResponseDto>().MessageSuccess(new CheckoutResponseDto
            {
                order_id = orderId.Value,
                order_code = orderCode,
                event_id = hold.event_id,
                event_name = hold.event_name ?? string.Empty,
                final_amount = totalAmount,
                paid_at = now,
                tickets = ticketItems
            }, "Thanh toán thành công");
        }
        catch (Exception e)
        {
            // Rollback toàn bộ nếu bất kỳ bước nào thất bại
            await seatHoldUnitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<CheckoutResponseDto>().MessageError($"Lỗi khi thanh toán: {e.Message}");
        }
    }

    private async Task<ResponseMessage<BookingHoldSeatDto>> ProcessHoldSeat(
        BookingHoldSeatRequest request, long userId, CancellationToken cancellationToken)
    {
        try
        {
            await seatHoldUnitOfWork.OpenAsync(cancellationToken);

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