namespace Ticketing.Infrastructure.Entities.SeatHold.Response;

public class SeatHoldEntity : BaseEntity
{
    public long hold_id { get; set; }
    public string hold_code { get; set; } = string.Empty;
    public long event_id { get; set; }
    public long customer_id { get; set; }
    public string status { get; set; } = string.Empty; // active, expired, released, converted, cancelled
    public DateTime hold_started_at { get; set; }
    public DateTime hold_expires_at { get; set; }
    public DateTime? released_at { get; set; }
    public string? release_reason { get; set; }
}

