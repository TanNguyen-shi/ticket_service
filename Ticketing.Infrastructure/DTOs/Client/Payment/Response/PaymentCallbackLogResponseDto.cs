namespace Ticketing.Infrastructure.DTOs.Client.Payment.Response;

/// <summary>
/// Response DTO cho PaymentCallbackLog GetById
/// </summary>
public class PaymentCallbackLogResponseDto
{
    public long callback_log_id { get; set; }
    public long payment_id { get; set; }
    public string? payment_ref { get; set; }
    public string? provider_transaction_ref { get; set; }
    public long order_id { get; set; }
    public string? order_code { get; set; }

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
/// Response DTO cho PaymentCallbackLog GetPagedList
/// </summary>
public class PaymentCallbackLogPagedDto
{
    public int row_index { get; set; }
    public int row_total { get; set; }

    public long callback_log_id { get; set; }
    public long payment_id { get; set; }
    public string? payment_ref { get; set; }
    public string? provider_transaction_ref { get; set; }
    public long order_id { get; set; }
    public string? order_code { get; set; }

    public string payment_provider { get; set; } = string.Empty;
    public string? external_transaction_ref { get; set; }
    public bool signature_valid { get; set; }
    public string processed_status { get; set; } = string.Empty;
    public DateTime received_at { get; set; }
    public DateTime? processed_at { get; set; }
}

