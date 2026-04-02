namespace Ticketing.Infrastructure.DTOs.EventZone.Request;

public class EventZoneGetPagedListRequest : BaseFilterPaging
{
    public long event_id { get; set; } = -1;
    public string? status { get; set; } = string.Empty;
}

