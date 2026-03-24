using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.VenueSection.Request;

public class VenueSectionGetByIdRequest
{
    [Required(ErrorMessage = "section_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "section_id must be greater than 0")]
    public long section_id { get; set; }
}

