using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.Venue.Request;

public class VenueDeleteRequest
{
    [Required(ErrorMessage = "venue_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "venue_id must be greater than 0")]
    public long venue_id { get; set; }

    public long? deleted_by { get; set; }
}