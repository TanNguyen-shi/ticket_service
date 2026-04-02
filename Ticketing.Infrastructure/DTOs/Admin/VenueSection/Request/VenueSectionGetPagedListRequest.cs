namespace Ticketing.Infrastructure.DTOs.VenueSection.Request;

public class VenueSectionGetPagedListRequest : BaseFilterPaging
{
    public long venue_id { get; set; }
    public string? status { get; set; } = "active";
}

