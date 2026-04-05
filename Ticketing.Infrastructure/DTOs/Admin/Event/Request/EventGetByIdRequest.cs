using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.Admin.Event.Request;

public class EventGetByIdRequest
{
    [Required(ErrorMessage = "event_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "event_id phải lớn hơn 0")]
    public long event_id { get; set; }
}

