using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.UseCases.Client.Booking.Interfaces;
using Ticketing.Infrastructure.DTOs.Client.Booking.Request;
using Ticketing.Infrastructure.Helpers.Interfaces;

namespace TicketingSystem.Controllers.Client;

[Route("api/client/booking")]
[ApiController]
[Authorize]
public class BookingController(IBookingUseCases bookingUseCase, IUserHelper user) : ControllerBase
{
    [HttpPost("hold-seat")]
    public async Task<IActionResult> HoldSeat([FromBody] BookingHoldSeatRequest request, CancellationToken cancellationToken = default)
    {
        var result = await bookingUseCase.HoldSeat(request, user.UserId ?? 0, cancellationToken);
        return Ok(result);
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request, CancellationToken cancellationToken = default)
    {
        var result = await bookingUseCase.CheckoutAsync(request, user.UserId ?? 0, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Huỷ phiên giữ chỗ — client bấm back từ trang checkout.
    /// Nhả toàn bộ ghế trong phiên về trạng thái available.
    /// </summary>
    [HttpDelete("release/{holdId:long}")]
    public async Task<IActionResult> ReleaseHold(long holdId, CancellationToken cancellationToken = default)
    {
        var result = await bookingUseCase.ReleaseHoldAsync(holdId, user.UserId ?? 0, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// [DEV ONLY] Kích hoạt thủ công job nhả ghế hết hạn — dùng để test, xoá trước khi lên prod.
    /// </summary>
    [HttpPost("dev/trigger-expire")]
    [AllowAnonymous]
    public async Task<IActionResult> TriggerExpire(CancellationToken cancellationToken = default)
    {
        await bookingUseCase.ReleaseExpiredHoldsAsync(cancellationToken);
        return Ok(new { message = "Đã kích hoạt release expired holds" });
    }
}
