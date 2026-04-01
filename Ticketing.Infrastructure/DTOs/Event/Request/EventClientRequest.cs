namespace Ticketing.Infrastructure.DTOs.Event.Request;

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

/// <summary>
/// Request model for searching events with filters
/// </summary>
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
    public string? keysearch { get; set; }

    /// <summary>
    /// Filter by event status (published, on_sale, ended)
    /// </summary>
    public string? status { get; set; }

    /// <summary>
    /// Filter by venue ID (-1 = all venues)
    /// </summary>
    public long venue_id { get; set; } = -1;

    /// <summary>
    /// Filter by featured flag (null = no filter)
    /// </summary>
    public bool? is_featured { get; set; }

    /// <summary>
    /// Filter by trending flag (null = no filter)
    /// </summary>
    public bool? is_trending { get; set; }

    /// <summary>
    /// Filter events from this date (ISO 8601 format)
    /// </summary>
    public DateTime? from_date { get; set; }

    /// <summary>
    /// Filter events to this date (ISO 8601 format)
    /// </summary>
    public DateTime? to_date { get; set; }
}

