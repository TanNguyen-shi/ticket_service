namespace Ticketing.Infrastructure.Entities.Idempotency.Response;

public class IdempotencyRequestEntity : BaseEntity
{
    public long idempotency_id { get; set; }
    public string idempotency_key { get; set; } = string.Empty;
    public string request_type { get; set; } = string.Empty; // hold_seats, payment, etc.
    public long user_id { get; set; }
    public string? request_hash { get; set; }
    public string status { get; set; } = string.Empty; // processing, completed, failed, expired
    public string? response_snapshot { get; set; } // JSON response stored
    public DateTime expired_at { get; set; }
}

