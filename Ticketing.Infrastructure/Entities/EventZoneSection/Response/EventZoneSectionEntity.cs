namespace Ticketing.Infrastructure.Entities.EventZoneSection.Response;

public class EventZoneSectionEntity : BaseEntity
{
    public long event_zone_section_id { get; set; }
    public long event_id { get; set; }
    public long event_zone_id { get; set; }
    public long section_id { get; set; }
}

