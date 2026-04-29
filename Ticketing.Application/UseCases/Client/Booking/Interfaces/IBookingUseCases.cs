using Ticketing.Application.Model.DTOs;
using Ticketing.Infrastructure.DTOs.Client.Booking.Request;
using Ticketing.Infrastructure.DTOs.Client.Booking.Response;

namespace Ticketing.Application.UseCases.Client.Booking.Interfaces;

public interface IBookingUseCases
{
    Task<ResponseMessage<BookingHoldSeatDto>> HoldSeat(BookingHoldSeatRequest request, long userId, CancellationToken cancellationToken = default);
    Task<ResponseMessage<CheckoutResponseDto>> CheckoutAsync(CheckoutRequest request, long customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Client chủ động huỷ phiên giữ chỗ (bấm back từ trang checkout).
    /// Nhả toàn bộ ghế trong phiên về trạng thái "available".
    /// </summary>
    Task<ResponseMessage<ReleaseHoldResponseDto>> ReleaseHoldAsync(long holdId, long customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Nhả tất cả phiên giữ chỗ đã hết hạn 10 phút.
    /// Gọi bởi background job, không cần customerId.
    /// </summary>
    Task ReleaseExpiredHoldsAsync(CancellationToken cancellationToken = default);
}