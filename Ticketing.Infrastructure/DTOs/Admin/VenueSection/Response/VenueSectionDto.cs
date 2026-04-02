namespace Ticketing.Infrastructure.DTOs.VenueSection.Response;

public class VenueSectionDto : AuditTable
{
    public long section_id { get; set; }
    public long venue_id { get; set; }
    public string? venue_code { get; set; }
    public string? venue_name { get; set; }
    public string section_code { get; set; } = string.Empty;
    public string section_name { get; set; } = string.Empty;
    public int display_order { get; set; }
    public string status { get; set; } = string.Empty;
}

