namespace Ticketing.Infrastructure.Entities.EventZone.Response;

public class EventZoneEntity : BaseEntity
{
    public long event_zone_id { get; set; }
    public long event_id { get; set; }
    public string zone_code { get; set; } = string.Empty;
    public string zone_name { get; set; } = string.Empty;
    public string? color_hex { get; set; }
    public string? description { get; set; }
    public int display_order { get; set; }
    public string status { get; set; } = "active";
}

