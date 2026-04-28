using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.UseCases.Client.Ticket.Interfaces;
using Ticketing.Infrastructure.Helpers.Interfaces;

namespace TicketingSystem.Controllers.Client;

[Route("api/client/ticket")]
[ApiController]
[Authorize]
public class TicketController(ITicketClientUseCases ticketUseCases, IUserHelper user) : ControllerBase
{
    /// <summary>
    /// Lấy danh sách vé của khách hàng đang đăng nhập
    /// </summary>
    [HttpGet("my-tickets")]
    public async Task<IActionResult> GetMyTickets(CancellationToken cancellationToken = default)
    {
        var result = await ticketUseCases.GetMyTicketsAsync(user.UserId ?? 0, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Lấy chi tiết một vé theo ticket_id (chỉ xem được vé của chính mình)
    /// </summary>
    [HttpGet("detail")]
    public async Task<IActionResult> GetDetail([FromQuery] long ticket_id, CancellationToken cancellationToken = default)
    {
        var result = await ticketUseCases.GetTicketDetailAsync(ticket_id, user.UserId ?? 0, cancellationToken);
        return Ok(result);
    }
}
