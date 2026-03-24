namespace Ticketing.Infrastructure.DTOs.VenueSeat.Request;

public class VenueSeatGetPagedListRequest : BaseFilterPaging
{
    public long venue_id { get; set; }
    public long section_id { get; set; }
    public string? status { get; set; } = "active";
    public string? seat_type { get; set; } = string.Empty;
}

