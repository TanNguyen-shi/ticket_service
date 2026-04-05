namespace Ticketing.Infrastructure.Entities.SeatHold.Response;

public class SeatHoldItemEntity : BaseEntity
{
    public long hold_item_id { get; set; }
    public long hold_id { get; set; }
    public long event_seat_inventory_id { get; set; }
    public long seat_id { get; set; }
    public long zone_id { get; set; }
    public decimal price_at_hold { get; set; }
    public string? seat_label_snapshot { get; set; }
    public string? zone_name_snapshot { get; set; }
    public string status { get; set; } = string.Empty; // active, released, converted, expired
}

