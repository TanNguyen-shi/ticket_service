using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.VenueSection.Request;

public class VenueSectionDeleteRequest
{
    [Required(ErrorMessage = "section_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "section_id phải lớn hơn 0")]
    public long section_id { get; set; }
}

