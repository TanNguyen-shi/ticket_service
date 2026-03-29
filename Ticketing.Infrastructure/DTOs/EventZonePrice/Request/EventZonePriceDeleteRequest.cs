using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.EventZonePrice.Request;

public class EventZonePriceDeleteRequest
{
    [Required(ErrorMessage = "event_zone_price_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "event_zone_price_id phải lớn hơn 0")]
    public long event_zone_price_id { get; set; }
}

