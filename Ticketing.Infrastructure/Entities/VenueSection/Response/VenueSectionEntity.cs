namespace Ticketing.Infrastructure.Entities.VenueSection.Response;

public class VenueSectionEntity : BaseEntity
{
    public long section_id { get; set; }
    public long venue_id { get; set; }
    public string section_code { get; set; } = string.Empty;
    public string section_name { get; set; } = string.Empty;
    public int display_order { get; set; }
    public string status { get; set; } = "active";
}

