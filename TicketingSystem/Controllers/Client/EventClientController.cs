using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.UseCases.Client.Event.Interfaces;
using Ticketing.Application.UseCases.Event.Interfaces;
using Ticketing.Infrastructure.DTOs.Admin.Event.Request;
using Ticketing.Infrastructure.DTOs.Client.Event.Request;
using Ticketing.Infrastructure.DTOs.Event.Request;
using Ticketing.Infrastructure.DTOs.Event.Response;

namespace TicketingSystem.Controllers.Client;

/// <summary>
/// Client Event Controller - Dùng cho Homepage, Explore page, ...
/// Endpoints không yêu cầu authentication
/// </summary>
[Route("api/client/event")]
[ApiController]
public class EventClientController(IEventClientUseCases eventUseCases) : ControllerBase
{
    /// <summary>
    /// Get Featured Events for Homepage
    /// </summary>
    /// <param name="request">Request with limit parameter</param>
    /// <remarks>
    /// Lấy danh sách sự kiện nổi bật được publish/on_sale
    /// Sắp xếp theo display_order, start_time
    /// </remarks>
    [HttpGet("featured")]
    public async Task<IActionResult> GetFeatured([FromQuery] EventGetFeaturedClientRequest request, CancellationToken cancellationToken = default)
    {
        var result = await eventUseCases.GetFeaturedAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get Trending Events for Homepage
    /// </summary>
    /// <param name="request">Request with limit parameter</param>
    /// <remarks>
    /// Lấy danh sách sự kiện xu hướng được publish/on_sale
    /// Ưu tiên các sự kiện nổi bật trước
    /// Sắp xếp theo display_order, start_time
    /// </remarks>
    [HttpGet("trending")]
    public async Task<IActionResult> GetTrending([FromQuery] EventGetTrendingClientRequest request, CancellationToken cancellationToken = default)
    {
        var result = await eventUseCases.GetTrendingAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get Upcoming Events for Homepage
    /// </summary>
    /// <param name="request">Request with limit parameter</param>
    /// <remarks>
    /// Lấy danh sách sự kiện sắp tới (start_time >= now)
    /// Ưu tiên sự kiện nổi bật/xu hướng
    /// Sắp xếp theo display_order, start_time
    /// </remarks>
    [HttpGet("upcoming")]
    public async Task<IActionResult> GetUpcoming([FromQuery] EventGetUpcomingClientRequest request, CancellationToken cancellationToken = default)
    {
        var result = await eventUseCases.GetUpcomingAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Search Events - Advanced filter for Explore/Discovery page
    /// </summary>
    /// <param name="request">Search request with all filter parameters</param>
    /// <remarks>
    /// Tất cả query params đều optional
    /// Response trả về danh sách sự kiện + metadata pagination
    /// </remarks>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] EventSearchClientRequest request, CancellationToken cancellationToken = default)
    {
        var result = await eventUseCases.SearchAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("detail")]
    public async Task<IActionResult> Detail([FromQuery] EventGetByIdRequest request, CancellationToken cancellationToken = default)
    {
        var result = await eventUseCases.GetDetailEvent(request, cancellationToken);
        return Ok(result);
    }
}