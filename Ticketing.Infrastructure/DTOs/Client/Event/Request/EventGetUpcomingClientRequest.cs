namespace Ticketing.Infrastructure.DTOs.Client.Event.Request;

/// <summary>
/// Request model for getting upcoming events
/// </summary>
public class EventGetUpcomingClientRequest
{
    /// <summary>
    /// Number of upcoming events to retrieve (default: 12)
    /// </summary>
    public int limit { get; set; } = 12;
}