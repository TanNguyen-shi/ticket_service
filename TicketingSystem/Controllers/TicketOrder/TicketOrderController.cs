using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.UseCases.TicketOrder.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.TicketOrder.Request;
using Ticketing.Infrastructure.DTOs.TicketOrder.Response;
using Ticketing.Infrastructure.Helpers.Interfaces;

namespace TicketingSystem.Controllers.TicketOrder;

[ApiController]
[Route("api/admin/event/order")]
[Authorize]
public class TicketOrderController(ITicketOrderUseCases ticketOrderUseCases, IUserHelper user) : ControllerBase
{
    [HttpPost("insert")]
    [ProducesResponseType(typeof(ResponseMessage<int>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Insert([FromBody] TicketOrderCreateRequest request, CancellationToken cancellationToken)
        => Ok(await ticketOrderUseCases.InsertAsync(request, user.UserId, cancellationToken));

    [HttpPut("update")]
    [ProducesResponseType(typeof(ResponseMessage<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromBody] TicketOrderUpdateRequest request, CancellationToken cancellationToken)
        => Ok(await ticketOrderUseCases.UpdateAsync(request, user.UserId, cancellationToken));

    [HttpDelete("delete")]
    [ProducesResponseType(typeof(ResponseMessage<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromBody] TicketOrderDeleteRequest request, CancellationToken cancellationToken)
        => Ok(await ticketOrderUseCases.DeleteAsync(request, user.UserId, cancellationToken));

    [HttpGet("getbyid")]
    [ProducesResponseType(typeof(ResponseMessage<TicketOrderDetailDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById([FromQuery] TicketOrderGetByIdRequest request, CancellationToken cancellationToken)
        => Ok(await ticketOrderUseCases.GetByIdAsync(request, user.UserId, cancellationToken));

    [HttpGet("getpagedlist")]
    [ProducesResponseType(typeof(ResponseMessage<IEnumerable<TicketOrderListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedList([FromQuery] TicketOrderGetPagedListRequest request, CancellationToken cancellationToken)
        => Ok(await ticketOrderUseCases.GetPagedListAsync(request, user.UserId, cancellationToken));
}

