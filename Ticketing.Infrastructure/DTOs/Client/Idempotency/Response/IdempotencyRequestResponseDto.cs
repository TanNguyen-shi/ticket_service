namespace Ticketing.Infrastructure.DTOs.Client.Idempotency.Response;

/// <summary>
/// Response DTO cho Idempotency GetById
/// </summary>
public class IdempotencyRequestResponseDto
{
    public long idempotency_id { get; set; }
    public string idempotency_key { get; set; } = string.Empty;
    public string request_type { get; set; } = string.Empty;
    public long user_id { get; set; }
    public string? username { get; set; }
    public string? full_name { get; set; }
    public string? request_hash { get; set; }
    public string status { get; set; } = string.Empty;
    public string? response_snapshot { get; set; }
    public DateTime created_at { get; set; }
    public DateTime expired_at { get; set; }
}

/// <summary>
/// Response DTO cho Idempotency GetPagedList
/// </summary>
public class IdempotencyRequestPagedDto
{
    public int row_index { get; set; }
    public int row_total { get; set; }
    public long idempotency_id { get; set; }
    public string idempotency_key { get; set; } = string.Empty;
    public string request_type { get; set; } = string.Empty;
    public long user_id { get; set; }
    public string? username { get; set; }
    public string? full_name { get; set; }
    public string? request_hash { get; set; }
    public string status { get; set; } = string.Empty;
    public string? response_snapshot { get; set; }
    public DateTime created_at { get; set; }
    public DateTime expired_at { get; set; }
}

