using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.EventPublishLog.Request;

public class EventPublishLogGetByIdRequest
{
    [Required(ErrorMessage = "event_publish_log_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "event_publish_log_id must be greater than 0")]
    public long event_publish_log_id { get; set; }
}

