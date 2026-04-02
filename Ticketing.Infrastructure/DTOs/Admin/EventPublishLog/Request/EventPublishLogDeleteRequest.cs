using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.EventPublishLog.Request;

public class EventPublishLogDeleteRequest
{
    [Required(ErrorMessage = "event_publish_log_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "event_publish_log_id phải lớn hơn 0")]
    public long event_publish_log_id { get; set; }
}

