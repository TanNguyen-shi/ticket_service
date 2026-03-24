using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.UseCases.VenueSection.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.VenueSection.Request;
using Ticketing.Infrastructure.DTOs.VenueSection.Response;
using Ticketing.Infrastructure.Helpers.Interfaces;

namespace TicketingSystem.Controllers.VenueSection;

[ApiController]
[Route("api/admin/venue-section")]
[Authorize]
public class VenueSectionController(IVenueSectionUseCases venueSectionUseCases, IUserHelper user) : ControllerBase
{
    [HttpPost("insert")]
    [ProducesResponseType(typeof(ResponseMessage<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseMessage<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Insert(
        [FromBody] VenueSectionCreateRequest request,
        CancellationToken cancellationToken)
    {
        var result = await venueSectionUseCases.InsertAsync(request, user.UserId, cancellationToken);

        if (result is null)
        {
            return BadRequest(ResponseMessage<object>.Error("Tạo mới khu vực thất bại"));
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
    public async Task<IActionResult> Update(
        [FromBody] VenueSectionUpdateRequest request,
        CancellationToken cancellationToken)
    {
        var result = await venueSectionUseCases.UpdateAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("delete")]
    [ProducesResponseType(typeof(ResponseMessage<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseMessage<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(
        [FromBody] VenueSectionDeleteRequest request,
        CancellationToken cancellationToken)
    {
        var result = await venueSectionUseCases.DeleteAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("getbyid")]
    [ProducesResponseType(typeof(ResponseMessage<VenueSectionDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseMessage<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById(
        [FromQuery] VenueSectionGetByIdRequest request,
        CancellationToken cancellationToken)
    {
        var result = await venueSectionUseCases.GetByIdAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("getpagedlist")]
    [ProducesResponseType(typeof(ResponseMessage<IEnumerable<VenueSectionListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseMessage<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPagedList(
        [FromQuery] VenueSectionGetPagedListRequest request,
        CancellationToken cancellationToken)
    {
        var result = await venueSectionUseCases.GetPagedListAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }
}

