namespace Ticketing.Infrastructure.DTOs.EventZoneSection.Response;

public class EventZoneSectionListDto : AuditTable
{
    public long event_zone_section_id { get; set; }
    public long event_id { get; set; }
    public string? event_code { get; set; }
    public string? event_name { get; set; }
    public long event_zone_id { get; set; }
    public string? zone_code { get; set; }
    public string? zone_name { get; set; }
    public long section_id { get; set; }
    public string? section_code { get; set; }
    public string? section_name { get; set; }
    public long row_index { get; set; }
    public long row_total { get; set; }
}

