using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.UseCases.TicketOrderItem.Interfaces;
using Ticketing.Infrastructure.DTOs.TicketOrderItem.Request;
using Ticketing.Infrastructure.Helpers.Interfaces;

namespace TicketingSystem.Controllers.Admin.Ticket;

[Route("api/admin/event/order-item")]
public class TicketOrderItemController(ITicketOrderItemUseCases ticketOrderItemUseCases, IUserHelper user) : BaseApiController
{
    [HttpPost("insert")]
    public async Task<IActionResult> Insert([FromBody] TicketOrderItemCreateRequest request, CancellationToken cancellationToken)
    {
        var result = await ticketOrderItemUseCases.InsertAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] TicketOrderItemUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await ticketOrderItemUseCases.UpdateAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromBody] TicketOrderItemDeleteRequest request, CancellationToken cancellationToken)
    {
        var result = await ticketOrderItemUseCases.DeleteAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("getbyid")]
    public async Task<IActionResult> GetById([FromQuery] TicketOrderItemGetByIdRequest request, CancellationToken cancellationToken)
    {
        var result = await ticketOrderItemUseCases.GetByIdAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("getpagedlist")]
    public async Task<IActionResult> GetPagedList([FromQuery] TicketOrderItemGetPagedListRequest request, CancellationToken cancellationToken)
    {
        var result = await ticketOrderItemUseCases.GetPagedListAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }
}
