using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.EventZone.Request;

public class EventZoneDeleteRequest
{
    [Required(ErrorMessage = "event_zone_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "event_zone_id phải lớn hơn 0")]
    public long event_zone_id { get; set; }
}

