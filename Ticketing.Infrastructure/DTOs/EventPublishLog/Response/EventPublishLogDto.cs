namespace Ticketing.Infrastructure.DTOs.EventPublishLog.Response;

public class EventPublishLogDto
{
    public long event_publish_log_id { get; set; }
    public long event_id { get; set; }
    public string? event_code { get; set; }
    public string? event_name { get; set; }
    public string action { get; set; } = string.Empty;
    public string? old_status { get; set; }
    public string? new_status { get; set; }
    public long? changed_by { get; set; }
    public string? changed_by_name { get; set; }
    public DateTime changed_at { get; set; }
    public string? note { get; set; }
}

