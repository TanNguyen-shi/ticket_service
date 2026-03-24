namespace Ticketing.Infrastructure.Entities.Venue.Response;

public class VenueEntity : BaseEntity
{
    public long venue_id { get; set; }
    public string venue_code { get; set; } = string.Empty;
    public string venue_name { get; set; } = string.Empty;
    public string? address_line { get; set; }
    public string? city { get; set; }
    public string? country { get; set; }
    public string status { get; set; } = string.Empty;
}