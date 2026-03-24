using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.UseCases.Event.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.Event.Request;
using Ticketing.Infrastructure.DTOs.Event.Response;
using Ticketing.Infrastructure.Helpers.Interfaces;

namespace TicketingSystem.Controllers.Event;

[ApiController]
[Route("api/admin/event")]
[Authorize]
public class EventController(IEventUseCases eventUseCases, IUserHelper user) : ControllerBase
{
    [HttpPost("insert")]
    [ProducesResponseType(typeof(ResponseMessage<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseMessage<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Insert([FromBody] EventCreateRequest request, CancellationToken cancellationToken)
    {
        var result = await eventUseCases.InsertAsync(request, user.UserId, cancellationToken);

        if (result is null)
        {
            return BadRequest(ResponseMessage<object>.Error("Tạo mới sự kiện thất bại"));
        }

        if (!result.issuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPut("update")]
    [ProducesResponseType(typeof(ResponseMessage<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseMessage<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update([FromBody] EventUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await eventUseCases.UpdateAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("delete")]
    [ProducesResponseType(typeof(ResponseMessage<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseMessage<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete([FromBody] EventDeleteRequest request, CancellationToken cancellationToken)
    {
        var result = await eventUseCases.DeleteAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("getbyid")]
    [ProducesResponseType(typeof(ResponseMessage<EventDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseMessage<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById([FromQuery] EventGetByIdRequest request, CancellationToken cancellationToken)
    {
        var result = await eventUseCases.GetByIdAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("getpagedlist")]
    [ProducesResponseType(typeof(ResponseMessage<IEnumerable<EventListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseMessage<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPagedList([FromQuery] EventGetPagedListRequest request, CancellationToken cancellationToken)
    {
        var result = await eventUseCases.GetPagedListAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }
}

