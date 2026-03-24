using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.UseCases.TicketOrderItem.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.TicketOrderItem.Request;
using Ticketing.Infrastructure.DTOs.TicketOrderItem.Response;
using Ticketing.Infrastructure.Helpers.Interfaces;

namespace TicketingSystem.Controllers.TicketOrderItem;

[ApiController]
[Route("api/admin/event/order-item")]
[Authorize]
public class TicketOrderItemController(ITicketOrderItemUseCases ticketOrderItemUseCases, IUserHelper user) : ControllerBase
{
    [HttpPost("insert")]
    [ProducesResponseType(typeof(ResponseMessage<int>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Insert([FromBody] TicketOrderItemCreateRequest request, CancellationToken cancellationToken)
        => Ok(await ticketOrderItemUseCases.InsertAsync(request, user.UserId, cancellationToken));

    [HttpPut("update")]
    [ProducesResponseType(typeof(ResponseMessage<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromBody] TicketOrderItemUpdateRequest request, CancellationToken cancellationToken)
        => Ok(await ticketOrderItemUseCases.UpdateAsync(request, user.UserId, cancellationToken));

    [HttpDelete("delete")]
    [ProducesResponseType(typeof(ResponseMessage<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromBody] TicketOrderItemDeleteRequest request, CancellationToken cancellationToken)
        => Ok(await ticketOrderItemUseCases.DeleteAsync(request, user.UserId, cancellationToken));

    [HttpGet("getbyid")]
    [ProducesResponseType(typeof(ResponseMessage<TicketOrderItemDetailDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById([FromQuery] TicketOrderItemGetByIdRequest request, CancellationToken cancellationToken)
        => Ok(await ticketOrderItemUseCases.GetByIdAsync(request, user.UserId, cancellationToken));

    [HttpGet("getpagedlist")]
    [ProducesResponseType(typeof(ResponseMessage<IEnumerable<TicketOrderItemListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedList([FromQuery] TicketOrderItemGetPagedListRequest request, CancellationToken cancellationToken)
        => Ok(await ticketOrderItemUseCases.GetPagedListAsync(request, user.UserId, cancellationToken));
}

