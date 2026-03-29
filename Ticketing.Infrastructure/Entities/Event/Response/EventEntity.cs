namespace Ticketing.Infrastructure.Entities.Event.Response;

public class EventEntity : BaseEntity
{
    public long event_id { get; set; }
    public string event_code { get; set; } = string.Empty;
    public string event_name { get; set; } = string.Empty;
    public string? description { get; set; }
    public long venue_id { get; set; }
    public string? banner_url { get; set; }
    public DateTime? start_time { get; set; }
    public DateTime? end_time { get; set; }
    public DateTime? sale_start_time { get; set; }
    public DateTime? sale_end_time { get; set; }
    public string status { get; set; } = "draft";
    public DateTime? published_at { get; set; }
    public DateTime? on_sale_at { get; set; }
}

