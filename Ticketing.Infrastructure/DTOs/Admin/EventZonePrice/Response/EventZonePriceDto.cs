namespace Ticketing.Infrastructure.DTOs.Admin.EventZonePrice.Response;

public class EventZonePriceDto : AuditTable
{
    public long event_zone_price_id { get; set; }
    public long event_zone_id { get; set; }
    public long? event_id { get; set; }
    public string? event_code { get; set; }
    public string? event_name { get; set; }
    public string? zone_code { get; set; }
    public string? zone_name { get; set; }
    public decimal price { get; set; }
    public string currency { get; set; } = string.Empty;
    public DateTime? start_time { get; set; }
    public DateTime? end_time { get; set; }
    public bool is_active { get; set; }
    public string? created_by_name { get; set; }
    public string? updated_by_name { get; set; }
}

