using Ticketing.Infrastructure.DTOs.Venue.Response;

namespace Ticketing.Infrastructure.DTOs.Admin.Venue.Response;

public class VenueListDto : VenueDto
{
    public long rowindex { get; set; }
    public long rowtotal { get; set; }
}