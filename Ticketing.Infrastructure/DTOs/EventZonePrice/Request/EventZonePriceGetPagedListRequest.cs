namespace Ticketing.Infrastructure.DTOs.EventZonePrice.Request;

public class EventZonePriceGetPagedListRequest : BaseFilterPaging
{
    public long event_zone_id { get; set; } = -1;
    public bool is_active { get; set; } = true;
}

