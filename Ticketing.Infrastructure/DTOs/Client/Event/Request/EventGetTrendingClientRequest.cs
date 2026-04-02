namespace Ticketing.Infrastructure.DTOs.Client.Event.Request;

/// <summary>
/// Request model for getting trending events
/// </summary>
public class EventGetTrendingClientRequest
{
    /// <summary>
    /// Number of trending events to retrieve (default: 12)
    /// </summary>
    public int limit { get; set; } = 12;
}