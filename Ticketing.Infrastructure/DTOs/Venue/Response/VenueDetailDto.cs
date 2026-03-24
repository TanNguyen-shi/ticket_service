namespace Ticketing.Infrastructure.DTOs.Venue.Response;

public class VenueDetailDto : VenueDto
{
    public long? created_by { get; set; }
    public DateTime created_at { get; set; }
    public long? updated_by { get; set; }
    public DateTime? updated_at { get; set; }
    public bool is_deleted { get; set; }
}