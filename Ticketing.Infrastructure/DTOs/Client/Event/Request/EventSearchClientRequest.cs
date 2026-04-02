namespace Ticketing.Infrastructure.DTOs.Client.Event.Request;

public class EventSearchClientRequest
{
    /// <summary>
    /// Number of records per page (default: 12)
    /// </summary>
    public int pagesize { get; set; } = 12;

    /// <summary>
    /// Page offset (default: 0)
    /// </summary>
    public int offset { get; set; } = 0;

    /// <summary>
    /// Search keyword - searches event_code, event_name, description, venue_name, city
    /// </summary>
    public string? keysearch { get; set; } = "";

    /// <summary>
    /// Filter events from this date (ISO 8601 format)
    /// </summary>
    public DateTime? from_date { get; set; } = DateTime.Now.AddMonths(-3);
    
    /// <summary>
    /// Filter events to this date (ISO 8601 format)
    /// </summary>
    public DateTime? to_date { get; set; } = DateTime.Now.AddMonths(1);
}