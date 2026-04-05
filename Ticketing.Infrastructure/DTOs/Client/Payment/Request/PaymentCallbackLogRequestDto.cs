namespace Ticketing.Infrastructure.DTOs.Client.Payment.Request;

/// <summary>
/// Request DTO cho PaymentCallbackLog Insert/Update
/// </summary>
public class PaymentCallbackLogInsertDto
{
    public long payment_id { get; set; }
    public string payment_provider { get; set; } = string.Empty;
    public string? external_transaction_ref { get; set; }
    public string? callback_signature { get; set; }
    public string? payload { get; set; }
    public bool signature_valid { get; set; }
    public string processed_status { get; set; } = "received";
    public DateTime received_at { get; set; }
    public DateTime? processed_at { get; set; }
}

/// <summary>
/// Request DTO cho PaymentCallbackLog Update
/// </summary>
public class PaymentCallbackLogUpdateDto
{
    public long callback_log_id { get; set; }
    public long payment_id { get; set; }
    public string payment_provider { get; set; } = string.Empty;
    public string? external_transaction_ref { get; set; }
    public string? callback_signature { get; set; }
    public string? payload { get; set; }
    public bool signature_valid { get; set; }
    public string processed_status { get; set; } = string.Empty;
    public DateTime received_at { get; set; }
    public DateTime? processed_at { get; set; }
}

/// <summary>
/// Request DTO cho PaymentCallbackLog Delete
/// </summary>
public class PaymentCallbackLogDeleteDto
{
    public long callback_log_id { get; set; }
}

/// <summary>
/// Request DTO cho PaymentCallbackLog GetById
/// </summary>
public class PaymentCallbackLogGetByIdDto
{
    public long callback_log_id { get; set; }
}

/// <summary>
/// Request DTO cho PaymentCallbackLog GetPagedList
/// </summary>
public class PaymentCallbackLogGetPagedListDto
{
    public int pagesize { get; set; } = 20;
    public int offset { get; set; } = 0;
    public string keysearch { get; set; } = string.Empty;
    public long payment_id { get; set; } = -1;
    public string payment_provider { get; set; } = string.Empty;
    public string processed_status { get; set; } = string.Empty;
}

