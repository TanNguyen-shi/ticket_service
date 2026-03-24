namespace Ticketing.Infrastructure.DTOs.EventSeatInventory.Request;

public class EventSeatInventoryGetPagedListRequest : BaseFilterPaging
{
    public long event_id { get; set; } = -1;
    public long event_zone_id { get; set; } = -1;
    public string? seat_status { get; set; } = string.Empty;
}

