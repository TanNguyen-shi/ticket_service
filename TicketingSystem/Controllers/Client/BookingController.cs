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
}
