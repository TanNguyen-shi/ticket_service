using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.UseCases.EventPublishLog.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.EventPublishLog.Request;
using Ticketing.Infrastructure.DTOs.EventPublishLog.Response;
using Ticketing.Infrastructure.Helpers.Interfaces;

namespace TicketingSystem.Controllers.EventPublishLog;

[ApiController]
[Route("api/admin/event_publish_log")]
[Authorize]
public class EventPublishLogController(IEventPublishLogUseCases eventPublishLogUseCases, IUserHelper user) : ControllerBase
{
    [HttpPost("insert")]
    [ProducesResponseType(typeof(ResponseMessage<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseMessage<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Insert([FromBody] EventPublishLogCreateRequest request, CancellationToken cancellationToken)
    {
        var result = await eventPublishLogUseCases.InsertAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }

    [HttpPut("update")]
    [ProducesResponseType(typeof(ResponseMessage<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseMessage<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update([FromBody] EventPublishLogUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await eventPublishLogUseCases.UpdateAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("delete")]
    [ProducesResponseType(typeof(ResponseMessage<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseMessage<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete([FromBody] EventPublishLogDeleteRequest request, CancellationToken cancellationToken)
    {
        var result = await eventPublishLogUseCases.DeleteAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("getbyid")]
    [ProducesResponseType(typeof(ResponseMessage<EventPublishLogDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseMessage<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById([FromQuery] EventPublishLogGetByIdRequest request, CancellationToken cancellationToken)
    {
        var result = await eventPublishLogUseCases.GetByIdAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("getpagedlist")]
    [ProducesResponseType(typeof(ResponseMessage<IEnumerable<EventPublishLogListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseMessage<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPagedList([FromQuery] EventPublishLogGetPagedListRequest request, CancellationToken cancellationToken)
    {
        var result = await eventPublishLogUseCases.GetPagedListAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }
}

