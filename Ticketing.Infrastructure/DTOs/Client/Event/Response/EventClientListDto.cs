namespace Ticketing.Infrastructure.DTOs.Client.Event.Response;

/// <summary>
/// Lightweight Event DTO cho phía Client (Homepage, Explore...)
/// Chỉ chứa thông tin cần thiết, không chứa audit fields
/// </summary>
public class EventClientListDto
{
    public long event_id { get; set; }
    public string event_code { get; set; } = string.Empty;
    public string event_name { get; set; } = string.Empty;
    public string? description { get; set; }
    
    // Venue Info
    public long venue_id { get; set; }
    public string? venue_code { get; set; }
    public string? venue_name { get; set; }
    public string? city { get; set; }
    public string? country { get; set; }
    
    // Event Details
    public string? banner_url { get; set; }
    public DateTime start_time { get; set; }
    public DateTime end_time { get; set; }
    public DateTime? sale_start_time { get; set; }
    public DateTime? sale_end_time { get; set; }
    public string status { get; set; } = string.Empty;
    public DateTime? published_at { get; set; }
    public DateTime? on_sale_at { get; set; }
    
    // New Fields
    public bool is_featured { get; set; } = false;
    public bool is_trending { get; set; } = false;
    public int display_order { get; set; } = 0;
}

