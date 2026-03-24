using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.Event.Request;

public class EventDeleteRequest
{
    [Required(ErrorMessage = "event_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "event_id must be greater than 0")]
    public long event_id { get; set; }
}

