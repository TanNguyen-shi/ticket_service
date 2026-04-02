namespace Ticketing.Infrastructure.DTOs.TicketOrderItem.Response;

public class TicketOrderItemDto
{
    public long order_item_id { get; set; }
    public long order_id { get; set; }
    public string? order_code { get; set; }
    public long event_seat_inventory_id { get; set; }
    public long? event_id { get; set; }
    public string? event_code { get; set; }
    public string? event_name { get; set; }
    public long seat_id { get; set; }
    public string? seat_code { get; set; }
    public string? seat_label { get; set; }
    public long zone_id { get; set; }
    public string? zone_code { get; set; }
    public string? zone_name { get; set; }
    public decimal unit_price { get; set; }
    public string? seat_label_snapshot { get; set; }
    public string? zone_name_snapshot { get; set; }
    public string item_status { get; set; } = string.Empty;
    public DateTime created_at { get; set; }
}

