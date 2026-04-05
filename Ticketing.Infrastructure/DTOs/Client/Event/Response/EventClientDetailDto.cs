namespace Ticketing.Infrastructure.DTOs.Client.Event.Response;

/// <summary>
/// Lightweight Event DTO cho phía Client (Homepage, Explore...)
/// Chỉ chứa thông tin cần thiết, không chứa audit fields
/// </summary>
public class EventClientDetailDto
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
    public bool is_featured { get; set; } = false;
    public bool is_trending { get; set; } = false;
    public int display_order { get; set; } = 0;

    public List<EventZoneDto> zones { get; set; } = new List<EventZoneDto>();
}

public class EventZoneDto
{
    public long event_zone_id { get; set; }
    public string zone_code { get; set; } = string.Empty;
    public string zone_name { get; set; } = string.Empty;
    public string? color_hex { get; set; }
    public string? description { get; set; }
    public int display_order { get; set; } = 0;
    public string status { get; set; } = string.Empty;
    public decimal current_price { get; set; }
    public List<EventVenueSeatDto> seats { get; set; } = new List<EventVenueSeatDto>();
}

public class EventVenueSeatDto
{
    public long seat_id { get; set; }
    public long section_id { get; set; }
    public decimal price { get; set; }
    public string row_label { get; set; }
    public string seat_number { get; set; }
    public string seat_label { get; set; }
    public decimal x_pos { get; set; }
    public decimal y_pos { get; set; }
    public string seat_type { get; set; }
    public string status { get; set; }
    public long? order_id { get; set; }
    public long? event_seat_inventory_id { get; set; }
}