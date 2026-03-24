using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.UseCases.VenueSeat.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.VenueSeat.Request;
using Ticketing.Infrastructure.DTOs.VenueSeat.Response;
using Ticketing.Infrastructure.Helpers.Interfaces;

namespace TicketingSystem.Controllers.VenueSeat;

[ApiController]
[Route("api/admin/venue-seat")]
[Authorize]
public class VenueSeatController(IVenueSeatUseCases venueSeatUseCases, IUserHelper user) : ControllerBase
{
    [HttpPost("insert")]
    [ProducesResponseType(typeof(ResponseMessage<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseMessage<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Insert([FromBody] VenueSeatCreateRequest request, CancellationToken cancellationToken)
    {
        var result = await venueSeatUseCases.InsertAsync(request, user.UserId, cancellationToken);

        if (result is null)
        {
            return BadRequest(ResponseMessage<object>.Error("Tạo mới ghế thất bại"));
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
    public async Task<IActionResult> Update([FromBody] VenueSeatUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await venueSeatUseCases.UpdateAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("delete")]
    [ProducesResponseType(typeof(ResponseMessage<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseMessage<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete([FromBody] VenueSeatDeleteRequest request, CancellationToken cancellationToken)
    {
        var result = await venueSeatUseCases.DeleteAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("getbyid")]
    [ProducesResponseType(typeof(ResponseMessage<VenueSeatDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseMessage<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById([FromQuery] VenueSeatGetByIdRequest request, CancellationToken cancellationToken)
    {
        var result = await venueSeatUseCases.GetByIdAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("getpagedlist")]
    [ProducesResponseType(typeof(ResponseMessage<IEnumerable<VenueSeatListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseMessage<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPagedList([FromQuery] VenueSeatGetPagedListRequest request, CancellationToken cancellationToken)
    {
        var result = await venueSeatUseCases.GetPagedListAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }
}

