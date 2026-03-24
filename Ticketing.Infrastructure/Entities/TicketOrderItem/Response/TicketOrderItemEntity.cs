namespace Ticketing.Infrastructure.Entities.TicketOrderItem.Response;

public class TicketOrderItemEntity : BaseEntity
{
    public long order_item_id { get; set; }
    public long order_id { get; set; }
    public long event_seat_inventory_id { get; set; }
    public long seat_id { get; set; }
    public long zone_id { get; set; }
    public decimal unit_price { get; set; }
    public string? seat_label_snapshot { get; set; }
    public string? zone_name_snapshot { get; set; }
    public string item_status { get; set; } = string.Empty;
}

