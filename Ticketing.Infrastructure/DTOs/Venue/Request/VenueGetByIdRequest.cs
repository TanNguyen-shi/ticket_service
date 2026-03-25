using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.Venue.Request;

public class VenueGetByIdRequest
{
    [Required(ErrorMessage = "venue_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "venue_id phải lớn hơn 0")]
    public long venue_id { get; set; }
}