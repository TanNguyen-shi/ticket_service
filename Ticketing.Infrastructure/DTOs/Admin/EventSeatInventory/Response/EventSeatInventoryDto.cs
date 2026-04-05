namespace Ticketing.Infrastructure.DTOs.Admin.EventSeatInventory.Response;

public class EventSeatInventoryDto
{
    public long event_seat_inventory_id { get; set; }
    public long event_id { get; set; }
    public string? event_code { get; set; }
    public string? event_name { get; set; }
    public long seat_id { get; set; }
    public string? seat_code { get; set; }
    public string? row_label { get; set; }
    public string? seat_number { get; set; }
    public string? seat_label { get; set; }
    public long event_zone_id { get; set; }
    public string? zone_code { get; set; }
    public string? zone_name { get; set; }
    public string seat_status { get; set; } = string.Empty;
    public long? current_hold_id { get; set; }
    public string? hold_code { get; set; }
    public long? current_order_item_id { get; set; }
    public string? order_item_code { get; set; }
    public decimal base_price { get; set; }
    public int version { get; set; }
    public DateTime updated_at { get; set; }
   
}

