using Ticketing.Infrastructure.DTOs;

namespace Ticketing.Infrastructure.DTOs.EventZoneSection.Request;

public class EventZoneSectionGetPagedListRequest : BaseFilterPaging
{
    public string keysearch { get; set; } = string.Empty;
    public long event_id { get; set; } = -1;
    public long event_zone_id { get; set; } = -1;
    public long section_id { get; set; } = -1;
}

