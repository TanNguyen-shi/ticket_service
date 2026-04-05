namespace Ticketing.Infrastructure.DTOs.Client.Payment.Request;

/// <summary>
/// Request DTO cho PaymentTransaction Insert/Update
/// </summary>
public class PaymentTransactionInsertDto
{
    public long order_id { get; set; }
    public string payment_provider { get; set; } = string.Empty;
    public string payment_ref { get; set; } = string.Empty;
    public string? provider_transaction_ref { get; set; }
    public decimal amount { get; set; }
    public string payment_status { get; set; } = "initiated";
    public DateTime requested_at { get; set; }
    public DateTime? confirmed_at { get; set; }
    public string? raw_request_payload { get; set; }
    public string? raw_callback_payload { get; set; }
}

/// <summary>
/// Request DTO cho PaymentTransaction Update
/// </summary>
public class PaymentTransactionUpdateDto
{
    public long payment_id { get; set; }
    public long order_id { get; set; }
    public string payment_provider { get; set; } = string.Empty;
    public string payment_ref { get; set; } = string.Empty;
    public string? provider_transaction_ref { get; set; }
    public decimal amount { get; set; }
    public string payment_status { get; set; } = string.Empty;
    public DateTime requested_at { get; set; }
    public DateTime? confirmed_at { get; set; }
    public string? raw_request_payload { get; set; }
    public string? raw_callback_payload { get; set; }
}

/// <summary>
/// Request DTO cho PaymentTransaction Check
/// </summary>
public class PaymentTransactionCheckDto
{
    public long payment_id { get; set; } = 0;
    public string payment_ref { get; set; } = string.Empty;
    public string? provider_transaction_ref { get; set; }
}

/// <summary>
/// Request DTO cho PaymentTransaction Delete
/// </summary>
public class PaymentTransactionDeleteDto
{
    public long payment_id { get; set; }
}

/// <summary>
/// Request DTO cho PaymentTransaction GetById
/// </summary>
public class PaymentTransactionGetByIdDto
{
    public long payment_id { get; set; }
}

/// <summary>
/// Request DTO cho PaymentTransaction GetPagedList
/// </summary>
public class PaymentTransactionGetPagedListDto
{
    public int pagesize { get; set; } = 20;
    public int offset { get; set; } = 0;
    public string keysearch { get; set; } = string.Empty;
    public long order_id { get; set; } = -1;
    public string payment_provider { get; set; } = string.Empty;
    public string payment_status { get; set; } = string.Empty;
}

