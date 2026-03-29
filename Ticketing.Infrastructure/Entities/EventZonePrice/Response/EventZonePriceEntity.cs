namespace Ticketing.Infrastructure.Entities.EventZonePrice.Response;

public class EventZonePriceEntity : BaseEntity
{
    public long event_zone_price_id { get; set; }
    public long event_zone_id { get; set; }
    public decimal price { get; set; }
    public string currency { get; set; } = string.Empty;
    public DateTime? start_time { get; set; }
    public DateTime? end_time { get; set; }
    public bool is_active { get; set; }
}

