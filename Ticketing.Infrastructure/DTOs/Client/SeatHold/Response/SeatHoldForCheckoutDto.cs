namespace Ticketing.Infrastructure.DTOs.Client.SeatHold.Response;

public class SeatHoldForCheckoutDto
{
    public long hold_id { get; set; }
    public string hold_code { get; set; } = string.Empty;
    public long event_id { get; set; }
    public string? event_name { get; set; }
    public long customer_id { get; set; }
    public string status { get; set; } = string.Empty;
    public DateTime hold_started_at { get; set; }
    public DateTime hold_expires_at { get; set; }
    public DateTime? released_at { get; set; }
    public string? release_reason { get; set; }
    public DateTime created_at { get; set; }
}

public class SeatHoldItemForCheckoutDto
{
    public long hold_item_id { get; set; }
    public long hold_id { get; set; }
    public long event_id { get; set; }
    public string? event_name { get; set; }
    public long event_seat_inventory_id { get; set; }
    public long seat_id { get; set; }
    public long zone_id { get; set; }
    public decimal price_at_hold { get; set; }
    public string seat_label_snapshot { get; set; } = string.Empty;
    public string zone_name_snapshot { get; set; } = string.Empty;
    public string status { get; set; } = string.Empty;
}
