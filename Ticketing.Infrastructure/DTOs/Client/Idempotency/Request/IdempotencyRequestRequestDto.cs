namespace Ticketing.Infrastructure.DTOs.Client.Idempotency.Request;

/// <summary>
/// Request DTO cho Idempotency Insert
/// </summary>
public class IdempotencyRequestInsertDto
{
    public string idempotency_key { get; set; } = string.Empty;
    public string request_type { get; set; } = string.Empty;
    public long user_id { get; set; }
    public string? request_hash { get; set; }
    public string status { get; set; } = "processing";
    public string? response_snapshot { get; set; }
    public DateTime expired_at { get; set; }
}

/// <summary>
/// Request DTO cho Idempotency Update
/// </summary>
public class IdempotencyRequestUpdateDto
{
    public long idempotency_id { get; set; }
    public string idempotency_key { get; set; } = string.Empty;
    public string request_type { get; set; } = string.Empty;
    public long user_id { get; set; }
    public string? request_hash { get; set; }
    public string status { get; set; } = string.Empty;
    public string? response_snapshot { get; set; }
    public DateTime expired_at { get; set; }
}

/// <summary>
/// Request DTO cho Idempotency Check
/// </summary>
public class IdempotencyRequestCheckDto
{
    public long idempotency_id { get; set; } = 0;
    public string idempotency_key { get; set; } = string.Empty;
    public string request_type { get; set; } = string.Empty;
}

/// <summary>
/// Request DTO cho Idempotency Delete
/// </summary>
public class IdempotencyRequestDeleteDto
{
    public long idempotency_id { get; set; }
}

/// <summary>
/// Request DTO cho Idempotency GetById
/// </summary>
public class IdempotencyRequestGetByIdDto
{
    public long idempotency_id { get; set; }
}

/// <summary>
/// Request DTO cho Idempotency GetPagedList
/// </summary>
public class IdempotencyRequestGetPagedListDto
{
    public int pagesize { get; set; } = 20;
    public int offset { get; set; } = 0;
    public string keysearch { get; set; } = string.Empty;
    public long user_id { get; set; } = -1;
    public string request_type { get; set; } = string.Empty;
    public string status { get; set; } = string.Empty;
}

