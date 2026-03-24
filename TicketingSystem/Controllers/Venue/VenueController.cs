using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.UseCases.Venue.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.Venue.Request;
using Ticketing.Infrastructure.DTOs.Venue.Response;
using Ticketing.Infrastructure.Helpers.Interfaces;

namespace TicketingSystem.Controllers.Venue;

[ApiController]
[Route("api/admin/venue")]
[Authorize]
public class VenueController(IVenueUseCases venueUseCases, IUserHelper user) : ControllerBase
{
    [HttpPost("insert")]
    [ProducesResponseType(typeof(ResponseMessage<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseMessage<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Insert(
        [FromBody] VenueCreateRequest request,
        CancellationToken cancellationToken)
    {

        var result = await venueUseCases.InsertAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    [HttpPut("update")]
    [ProducesResponseType(typeof(ResponseMessage<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseMessage<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update(
        [FromBody] VenueUpdateRequest request,
        CancellationToken cancellationToken)
    {
        var result = await venueUseCases.UpdateAsync(
            request,
            user.UserId,
            cancellationToken);


        return Ok(result);
    }

    [HttpDelete("delete")]
    [ProducesResponseType(typeof(ResponseMessage<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseMessage<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(
        [FromBody] VenueDeleteRequest request,
        CancellationToken cancellationToken)
    {

        var result = await venueUseCases.DeleteAsync(
            request,
            user.UserId,
            cancellationToken);


        return Ok(result);
    }

    [HttpGet("getbyid")]
    [ProducesResponseType(typeof(ResponseMessage<VenueDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseMessage<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById(
        [FromQuery] VenueGetByIdRequest request,
        CancellationToken cancellationToken)
    {

        var result = await venueUseCases.GetByIdAsync(
            request,
            user.UserId,
            cancellationToken);


        return Ok(result);
    }

    [HttpGet("getpagedlist")]
    [ProducesResponseType(typeof(ResponseMessage<IEnumerable<VenueListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseMessage<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPagedList(
        [FromQuery] VenueGetPagedListRequest request,
        CancellationToken cancellationToken)
    {
        var result = await venueUseCases.GetPagedListAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }
}