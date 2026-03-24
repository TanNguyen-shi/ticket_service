using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.UseCases.Ticket.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.Ticket.Request;
using Ticketing.Infrastructure.DTOs.Ticket.Response;
using Ticketing.Infrastructure.Helpers.Interfaces;

namespace TicketingSystem.Controllers.Ticket;

[ApiController]
[Route("api/admin/event/ticket")]
[Authorize]
public class TicketController(ITicketUseCases ticketUseCases, IUserHelper user) : ControllerBase
{
    [HttpPost("insert")]
    [ProducesResponseType(typeof(ResponseMessage<int>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Insert([FromBody] TicketCreateRequest request, CancellationToken cancellationToken)
        => Ok(await ticketUseCases.InsertAsync(request, user.UserId, cancellationToken));

    [HttpPut("update")]
    [ProducesResponseType(typeof(ResponseMessage<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromBody] TicketUpdateRequest request, CancellationToken cancellationToken)
        => Ok(await ticketUseCases.UpdateAsync(request, user.UserId, cancellationToken));

    [HttpDelete("delete")]
    [ProducesResponseType(typeof(ResponseMessage<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromBody] TicketDeleteRequest request, CancellationToken cancellationToken)
        => Ok(await ticketUseCases.DeleteAsync(request, user.UserId, cancellationToken));

    [HttpGet("getbyid")]
    [ProducesResponseType(typeof(ResponseMessage<TicketDetailDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById([FromQuery] TicketGetByIdRequest request, CancellationToken cancellationToken)
        => Ok(await ticketUseCases.GetByIdAsync(request, user.UserId, cancellationToken));

    [HttpGet("getpagedlist")]
    [ProducesResponseType(typeof(ResponseMessage<IEnumerable<TicketListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedList([FromQuery] TicketGetPagedListRequest request, CancellationToken cancellationToken)
        => Ok(await ticketUseCases.GetPagedListAsync(request, user.UserId, cancellationToken));
}

