namespace Ticketing.Infrastructure.DTOs.Client.Event.Request;

/// <summary>
/// Request model for getting featured events
/// </summary>
public class EventGetFeaturedClientRequest
{
    /// <summary>
    /// Number of featured events to retrieve (default: 8)
    /// </summary>
    public int limit { get; set; } = 8;
}