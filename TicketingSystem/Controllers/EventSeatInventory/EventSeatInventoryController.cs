using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.UseCases.EventSeatInventory.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.EventSeatInventory.Request;
using Ticketing.Infrastructure.DTOs.EventSeatInventory.Response;
using Ticketing.Infrastructure.Helpers.Interfaces;

namespace TicketingSystem.Controllers.EventSeatInventory;

[ApiController]
[Route("api/admin/event/seat-inventory")]
[Authorize]
public class EventSeatInventoryController(IEventSeatInventoryUseCases eventSeatInventoryUseCases, IUserHelper user) : ControllerBase
{
    [HttpPost("insert")]
    [ProducesResponseType(typeof(ResponseMessage<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseMessage<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Insert([FromBody] EventSeatInventoryCreateRequest request, CancellationToken cancellationToken)
    {
        var result = await eventSeatInventoryUseCases.InsertAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }

    [HttpPut("update")]
    [ProducesResponseType(typeof(ResponseMessage<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseMessage<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update([FromBody] EventSeatInventoryUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await eventSeatInventoryUseCases.UpdateAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("delete")]
    [ProducesResponseType(typeof(ResponseMessage<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseMessage<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete([FromBody] EventSeatInventoryDeleteRequest request, CancellationToken cancellationToken)
    {
        var result = await eventSeatInventoryUseCases.DeleteAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("getbyid")]
    [ProducesResponseType(typeof(ResponseMessage<EventSeatInventoryDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseMessage<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById([FromQuery] EventSeatInventoryGetByIdRequest request, CancellationToken cancellationToken)
    {
        var result = await eventSeatInventoryUseCases.GetByIdAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("getpagedlist")]
    [ProducesResponseType(typeof(ResponseMessage<IEnumerable<EventSeatInventoryListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseMessage<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPagedList([FromQuery] EventSeatInventoryGetPagedListRequest request, CancellationToken cancellationToken)
    {
        var result = await eventSeatInventoryUseCases.GetPagedListAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }
}
