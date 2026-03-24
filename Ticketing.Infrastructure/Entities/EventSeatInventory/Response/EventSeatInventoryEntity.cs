namespace Ticketing.Infrastructure.Entities.EventSeatInventory.Response;

public class EventSeatInventoryEntity : BaseEntity
{
    public long event_seat_inventory_id { get; set; }
    public long event_id { get; set; }
    public long seat_id { get; set; }
    public long event_zone_id { get; set; }
    public string seat_status { get; set; } = "available";
    public long? current_hold_id { get; set; }
    public long? current_order_item_id { get; set; }
    public decimal base_price { get; set; }
    public int version { get; set; }
    public DateTime updated_at { get; set; }
}

