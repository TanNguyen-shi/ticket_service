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
    /// <summary>
    /// Giữ chỗ cho khách hàng với bảo vệ idempotency.
    ///
    /// IDEMPOTENCY FLOW:
    ///   Client tự sinh một UUID (idempotency_key) trước mỗi lần bấm "Giữ chỗ".
    ///   Nếu request bị timeout hoặc mất mạng, client retry với cùng key đó.
    ///   Server dùng key để nhận biết retry và trả về kết quả cũ thay vì xử lý lại.
    ///
    ///   Key được scope theo: event + customer + client_uuid
    ///     → tránh 2 customer khác nhau dùng cùng UUID bị nhầm kết quả.
    ///
    /// TRẠNG THÁI IDEMPOTENCY RECORD:
    ///   processing  → đang xử lý (chặn retry nếu chưa hết hạn)
    ///   completed   → đã xong, trả cached response
    ///   failed      → thất bại, cho phép retry
    ///
    /// TRƯỜNG HỢP ĐẶC BIỆT CHO PHÉP RETRY:
    ///   - status = "failed"
    ///   - status = "processing" nhưng expired_at đã qua (server crash giữa chừng)
    ///   - status = "completed" nhưng response_snapshot bị hỏng/null
    /// </summary>
    public async Task<ResponseMessage<BookingHoldSeatDto>> HoldSeat(BookingHoldSeatRequest request, long customerId, CancellationToken cancellationToken = default)
    {
        if (customerId <= 0)
            return new ResponseMessage<BookingHoldSeatDto>().MessageError("Vui lòng đăng nhập để tiếp tục");

        // Key scope: event + customer + client_uuid → mỗi customer có không gian key riêng biệt
        var idempotencyKey = $"hold-evt{request.event_id}_cust{customerId}_{request.idempotency_key}";
        // Hash toàn bộ payload để phát hiện client dùng cùng key nhưng gửi request khác
        var requestHash = ComputeHash(request);
        long idempotencyIdLong = 0;

        try
        {
            // Bước 1: Tra cứu xem key này đã tồn tại trong DB chưa
            var existingRequest = await idempotencyRepository
                .GetByKeyAsync<IdempotencyRequestResponseDto>(new { idempotency_key = idempotencyKey }, cancellationToken);

            if (existingRequest is not null)
            {
                // Bước 2a: Key tồn tại — kiểm tra payload có khớp không
                // Nếu hash khác → client đang tái dùng key cho một request hoàn toàn khác (sai cách dùng idempotency)
                if (existingRequest.request_hash != requestHash)
                    return new ResponseMessage<BookingHoldSeatDto>().MessageError(
                        "Idempotency key này đã được sử dụng cho một yêu cầu khác, vui lòng dùng key mới");

                // Bước 2b: Hash khớp → xét trạng thái để quyết định xử lý tiếp thế nào
                switch (existingRequest.status)
                {
                    case "completed":
                        // Đã hoàn thành trước đó → trả về kết quả cache, không xử lý lại
                        var cachedResponse = string.IsNullOrEmpty(existingRequest.response_snapshot)
                            ? null
                            : JsonSerializer.Deserialize<BookingHoldSeatDto>(existingRequest.response_snapshot);
                        if (cachedResponse is not null)
                            return new ResponseMessage<BookingHoldSeatDto>().MessageSuccess(cachedResponse, "Yêu cầu đã được xử lý trước đó");
                        // response_snapshot bị null/hỏng dù status completed → xử lý lại (fall through)
                        break;

                    case "processing" when existingRequest.expired_at > DateTime.Now:
                        // Đang xử lý và chưa hết hạn → chặn retry, tránh chạy song song 2 request cùng key
                        return new ResponseMessage<BookingHoldSeatDto>().MessageError(
                            "Yêu cầu đang được xử lý, vui lòng thử lại sau ít giây");

                    // Các trường hợp fall through — đều được phép retry:
                    //   "failed"                          → request thất bại, cho phép thử lại
                    //   "processing" + expired_at đã qua → server crash giữa chừng, record kẹt ở processing
                    //   "completed"  + snapshot hỏng     → không có data để trả, buộc phải xử lý lại
                }

                // Bước 2c: Tái sử dụng record cũ, reset về "processing" để xử lý lại
                // Dùng UpdateAsync thay vì InsertAsync để tránh vi phạm unique constraint trên idempotency_key
                idempotencyIdLong = existingRequest.idempotency_id;
                await idempotencyRepository.UpdateAsync(new
                {
                    idempotency_id = idempotencyIdLong,
                    idempotency_key = idempotencyKey,
                    request_type = "hold_seats",
                    customer_id = customerId,
                    request_hash = requestHash,
                    status = "processing",
                    response_snapshot = string.Empty,
                    expired_at = DateTime.Now.AddMinutes(10)
                }, cancellationToken);
            }
            else
            {
                // Bước 2d: Key hoàn toàn mới → tạo record mới với status "processing"
                // expired_at = +10 phút: nếu server crash, record này sẽ được phép retry sau khoảng thời gian đó
                var idempotencyId = await idempotencyRepository.InsertAsync(new
                {
                    idempotency_key = idempotencyKey,
                    request_type = "hold_seats",
                    customer_id = customerId,
                    request_hash = requestHash,
                    status = "processing",
                    response_snapshot = string.Empty,
                    expired_at = DateTime.Now.AddMinutes(10)
                }, cancellationToken);

                if (string.IsNullOrEmpty(idempotencyId) || !long.TryParse(idempotencyId, out idempotencyIdLong))
                    return new ResponseMessage<BookingHoldSeatDto>().MessageError("Không thể tạo bản ghi idempotency, vui lòng thử lại");
            }

            // Bước 3: Thực hiện nghiệp vụ giữ chỗ (mở transaction, lock ghế, ghi DB)
            var processResult = await ProcessHoldSeat(request, customerId, cancellationToken);

            // Bước 4: Cập nhật kết quả vào idempotency record
            //   - Thành công → "completed" + lưu response_snapshot để trả về cho retry sau
            //   - Thất bại   → "failed" + snapshot = null (retry sẽ xử lý lại từ đầu)
            await idempotencyRepository.UpdateAsync(new
            {
                idempotency_id = idempotencyIdLong,
                idempotency_key = idempotencyKey,
                request_type = "hold_seats",
                customer_id = customerId,
                request_hash = requestHash,
                status = processResult.issuccess ? "completed" : "failed",
                response_snapshot = processResult.issuccess ? JsonSerializer.Serialize(processResult.data) : string.Empty,
                expired_at = DateTime.Now.AddMinutes(10)
            }, cancellationToken);

            return processResult.issuccess
                ? processResult
                : new ResponseMessage<BookingHoldSeatDto>().MessageError(processResult.message ?? "Giữ chỗ thất bại, vui lòng thử lại");
        }
        catch (Exception e)
        {
            // Bước 5 (exception path): Cố gắng đánh dấu record là "failed" để unblock retry sau này
            // Dùng try/catch riêng vì đây là best-effort — không để lỗi cleanup che lỗi gốc
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
                        response_snapshot = string.Empty,
                        expired_at = DateTime.Now.AddMinutes(10)
                    }, cancellationToken);
                }
                catch
                {
                    /* best effort — nếu update thất bại, record sẽ tự hết hạn và cho phép retry */
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

            if (hold.hold_expires_at <= DateTime.Now)
                return new ResponseMessage<CheckoutResponseDto>().MessageError("Phiên giữ chỗ đã hết hạn, vui lòng thực hiện lại");

            // Bước 5: Đọc danh sách ghế trong phiên giữ chỗ
            var holdItems = (await seatHoldUnitOfWork.SeatHoldItemRepository
                .GetByHoldIdAsync<SeatHoldItemForCheckoutDto>(new { hold_id = request.hold_id }, cancellationToken)).ToList();

            if (holdItems.Count == 0)
                return new ResponseMessage<CheckoutResponseDto>().MessageError("Phiên giữ chỗ không có thông tin ghế");

            // Bước 6: Bắt đầu transaction — tất cả các bước dưới phải thành công hoặc rollback toàn bộ
            await seatHoldUnitOfWork.BeginTransactionAsync(cancellationToken);

            var now = DateTime.Now;
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
                released_at = DateTime.Now,
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

            var holdStartedAt = DateTime.Now;
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

    /// <summary>
    /// Client chủ động huỷ phiên giữ chỗ (bấm back từ trang checkout).
    ///
    /// FLOW:
    ///   1. Lấy thông tin hold, xác thực quyền sở hữu và trạng thái
    ///   2. Lấy danh sách ghế trong phiên
    ///   3. Trong transaction:
    ///      - Reset từng ghế inventory: "held" → "available"
    ///      - Cập nhật toàn bộ seat_hold_item → "released"
    ///      - Cập nhật seat_hold → "released"
    /// </summary>
    public async Task<ResponseMessage<ReleaseHoldResponseDto>> ReleaseHoldAsync(
        long holdId,
        long customerId,
        CancellationToken cancellationToken = default)
    {
        if (customerId <= 0)
            return new ResponseMessage<ReleaseHoldResponseDto>().MessageError("Vui lòng đăng nhập để tiếp tục");

        if (holdId <= 0)
            return new ResponseMessage<ReleaseHoldResponseDto>().MessageError("Thông tin phiên giữ chỗ không hợp lệ");

        try
        {
            await seatHoldUnitOfWork.OpenAsync(cancellationToken);

            var hold = await seatHoldUnitOfWork.SeatHoldRepository
                .GetAsync<SeatHoldForCheckoutDto>(new { hold_id = holdId }, cancellationToken);

            if (hold is null)
                return new ResponseMessage<ReleaseHoldResponseDto>().MessageError("Không tìm thấy phiên giữ chỗ");

            if (hold.customer_id != customerId)
                return new ResponseMessage<ReleaseHoldResponseDto>().MessageError("Bạn không có quyền huỷ phiên giữ chỗ này");

            if (hold.status != "active")
                return new ResponseMessage<ReleaseHoldResponseDto>().MessageError("Phiên giữ chỗ không còn ở trạng thái active");

            var holdItems = (await seatHoldUnitOfWork.SeatHoldItemRepository
                .GetByHoldIdAsync<SeatHoldItemForCheckoutDto>(new { hold_id = holdId }, cancellationToken)).ToList();

            var releasedAt = DateTime.UtcNow;
            const string releaseReason = "Khách hàng huỷ giữ chỗ";

            await seatHoldUnitOfWork.BeginTransactionAsync(cancellationToken);

            // Reset từng ghế về "available"
            foreach (var item in holdItems)
            {
                await seatHoldUnitOfWork.EventSeatInventoryRepository.UpdateReleaseAsync(new
                {
                    event_seat_inventory_id = item.event_seat_inventory_id,
                    event_id = item.event_id,
                    seat_id = item.seat_id,
                    event_zone_id = item.zone_id
                }, cancellationToken);
            }

            // Cập nhật toàn bộ seat_hold_item → "released"
            await seatHoldUnitOfWork.SeatHoldItemRepository.UpdateStatusByHoldIdAsync(new
            {
                hold_id = holdId,
                status = "released"
            }, cancellationToken);

            // Cập nhật seat_hold → "released"
            await seatHoldUnitOfWork.SeatHoldRepository.UpdateAsync(new
            {
                hold_id = hold.hold_id,
                hold_code = hold.hold_code,
                event_id = hold.event_id,
                customer_id = hold.customer_id,
                status = "released",
                hold_started_at = hold.hold_started_at,
                hold_expires_at = hold.hold_expires_at,
                released_at = releasedAt,
                release_reason = releaseReason
            }, cancellationToken);

            await seatHoldUnitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return new ResponseMessage<ReleaseHoldResponseDto>().MessageSuccess(new ReleaseHoldResponseDto
            {
                hold_id = holdId,
                release_reason = releaseReason,
                released_at = releasedAt
            }, "Huỷ giữ chỗ thành công");
        }
        catch (Exception e)
        {
            await seatHoldUnitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<ReleaseHoldResponseDto>().MessageError($"Lỗi khi huỷ giữ chỗ: {e.Message}");
        }
    }

    /// <summary>
    /// Tự động nhả tất cả phiên giữ chỗ đã hết hạn 10 phút.
    /// Gọi bởi SeatHoldExpiryBackgroundService mỗi 60 giây.
    ///
    /// Mỗi hold được release trong transaction riêng biệt:
    /// nếu một hold thất bại, các hold khác vẫn tiếp tục xử lý.
    /// </summary>
    public async Task ReleaseExpiredHoldsAsync(CancellationToken cancellationToken = default)
    {
        await seatHoldUnitOfWork.OpenAsync(cancellationToken);

        var expiredHolds = (await seatHoldUnitOfWork.SeatHoldRepository
            .GetExpiredActiveAsync<ExpiredHoldDto>(cancellationToken)).ToList();

        if (expiredHolds.Count == 0)
            return;

        foreach (var expired in expiredHolds)
        {
            if (cancellationToken.IsCancellationRequested) break;
            try
            {
                await ProcessExpiredRelease(expired.hold_id, cancellationToken);
            }
            catch
            {
                // Một hold thất bại không ảnh hưởng đến các hold khác
                // Connection đã được đóng bởi RollbackAsync — vòng sau tiếp tục bình thường
            }
        }
    }

    /// <summary>
    /// Nhả một phiên giữ chỗ hết hạn, không cần xác thực quyền sở hữu.
    /// Chỉ dùng nội bộ bởi ReleaseExpiredHoldsAsync.
    /// </summary>
    private async Task ProcessExpiredRelease(long holdId, CancellationToken cancellationToken)
    {
        // Reads chạy trên connection riêng (DapperProcedureHelper tự tạo nếu contextAccessor.Connection = null)
        var hold = await seatHoldUnitOfWork.SeatHoldRepository
            .GetAsync<SeatHoldForCheckoutDto>(new { hold_id = holdId }, cancellationToken);

        // Guard: bỏ qua nếu hold đã được xử lý bởi luồng khác giữa chừng
        if (hold is null || hold.status != "active")
            return;

        var holdItems = (await seatHoldUnitOfWork.SeatHoldItemRepository
            .GetByHoldIdAsync<SeatHoldItemForCheckoutDto>(new { hold_id = holdId }, cancellationToken)).ToList();

        var releasedAt = DateTime.UtcNow;
        const string releaseReason = "Hết hạn giữ chỗ (10 phút)";

        try
        {
            // Transaction bắt đầu TẠI ĐÂY — reads ở trên chạy ngoài transaction là đúng
            await seatHoldUnitOfWork.BeginTransactionAsync(cancellationToken);

            foreach (var item in holdItems)
            {
                await seatHoldUnitOfWork.EventSeatInventoryRepository.UpdateReleaseAsync(new
                {
                    event_seat_inventory_id = item.event_seat_inventory_id,
                    event_id = item.event_id,
                    seat_id = item.seat_id,
                    event_zone_id = item.zone_id
                }, cancellationToken);
            }

            await seatHoldUnitOfWork.SeatHoldItemRepository.UpdateStatusByHoldIdAsync(new
            {
                hold_id = holdId,
                status = "released"
            }, cancellationToken);

            await seatHoldUnitOfWork.SeatHoldRepository.UpdateAsync(new
            {
                hold_id = hold.hold_id,
                hold_code = hold.hold_code,
                event_id = hold.event_id,
                customer_id = hold.customer_id,
                status = "released",
                hold_started_at = hold.hold_started_at,
                hold_expires_at = hold.hold_expires_at,
                released_at = releasedAt,
                release_reason = releaseReason
            }, cancellationToken);

            await seatHoldUnitOfWork.CommitAsync(cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            // RollbackAsync đóng connection và clear contextAccessor
            // → vòng lặp tiếp theo sẽ có connection sạch, tránh lỗi "transaction is aborted"
            await seatHoldUnitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            throw; // re-throw để ReleaseExpiredHoldsAsync log và tiếp tục hold tiếp theo
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