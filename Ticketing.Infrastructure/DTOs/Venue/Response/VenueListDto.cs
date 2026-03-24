namespace Ticketing.Infrastructure.DTOs.Venue.Response;

public class VenueListDto : VenueDto
{
    public long rowindex { get; set; }
    public long rowtotal { get; set; }
}