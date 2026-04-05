using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.EventZoneSection.Request;

public class EventZoneSectionGetByIdRequest
{
    [Required(ErrorMessage = "event_zone_section_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "event_zone_section_id phải lớn hơn 0")]
    public long event_zone_section_id { get; set; }
}

