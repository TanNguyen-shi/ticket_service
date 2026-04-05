using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.UseCases.Client.Booking.Interfaces;
using Ticketing.Application.UseCases.Client.Event.Interfaces;
using Ticketing.Application.UseCases.Event.Interfaces;
using Ticketing.Infrastructure.DTOs.Admin.Event.Request;
using Ticketing.Infrastructure.DTOs.Client.Booking.Request;
using Ticketing.Infrastructure.DTOs.Client.Event.Request;
using Ticketing.Infrastructure.DTOs.Event.Request;
using Ticketing.Infrastructure.DTOs.Event.Response;

namespace TicketingSystem.Controllers.Client;

/// <summary>
/// Client Event Controller - Dùng cho Homepage, Explore page, ...
/// Endpoints không yêu cầu authentication
/// </summary>
[Route("api/client/booking")]
[ApiController]
public class BookingController(IBookingUseCases bookingUseCase) : ControllerBase
{
    /// <summary>
    /// Get Featured Events for Homepage
    /// </summary>
    /// <param name="request">Request with limit parameter</param>
    /// <remarks>
    /// Lấy danh sách sự kiện nổi bật được publish/on_sale
    /// Sắp xếp theo display_order, start_time
    /// </remarks>
    [HttpPost("hold-seat")]
    public async Task<IActionResult> HoldSeat([FromBody] BookingHoldSeatRequest request, CancellationToken cancellationToken = default)
    {
        var result = await bookingUseCase.HoldSeat(request, cancellationToken);
        return Ok(result);
    }
}