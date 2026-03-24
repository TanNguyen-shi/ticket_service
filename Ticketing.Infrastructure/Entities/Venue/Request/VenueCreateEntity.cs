namespace Ticketing.Infrastructure.Entities.Venue.Request;

public class VenueCreateEntity
{
    public string venue_code { get; set; } = string.Empty;
    public string venue_name { get; set; } = string.Empty;
    public string? address_line { get; set; }
    public string? city { get; set; }
    public string? country { get; set; }
    public string status { get; set; } = string.Empty;
    public string? created_by { get; set; }
}