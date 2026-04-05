namespace Ticketing.Infrastructure.DTOs.Client.Payment.Response;

/// <summary>
/// Response DTO cho PaymentTransaction GetById
/// </summary>
public class PaymentTransactionResponseDto
{
    public long payment_id { get; set; }
    public long order_id { get; set; }
    public string? order_code { get; set; }
    public long event_id { get; set; }
    public string? event_code { get; set; }
    public string? event_name { get; set; }
    public long user_id { get; set; }
    public string? username { get; set; }
    public string? full_name { get; set; }

    public string payment_provider { get; set; } = string.Empty;
    public string payment_ref { get; set; } = string.Empty;
    public string? provider_transaction_ref { get; set; }
    public decimal amount { get; set; }
    public string payment_status { get; set; } = string.Empty;
    public DateTime requested_at { get; set; }
    public DateTime? confirmed_at { get; set; }
    public string? raw_request_payload { get; set; }
    public string? raw_callback_payload { get; set; }
    public DateTime created_at { get; set; }
    public DateTime? updated_at { get; set; }
}

/// <summary>
/// Response DTO cho PaymentTransaction GetPagedList
/// </summary>
public class PaymentTransactionPagedDto
{
    public int row_index { get; set; }
    public int row_total { get; set; }

    public long payment_id { get; set; }
    public long order_id { get; set; }
    public string? order_code { get; set; }
    public long event_id { get; set; }
    public string? event_code { get; set; }
    public string? event_name { get; set; }
    public long user_id { get; set; }
    public string? username { get; set; }
    public string? full_name { get; set; }

    public string payment_provider { get; set; } = string.Empty;
    public string payment_ref { get; set; } = string.Empty;
    public string? provider_transaction_ref { get; set; }
    public decimal amount { get; set; }
    public string payment_status { get; set; } = string.Empty;
    public DateTime requested_at { get; set; }
    public DateTime? confirmed_at { get; set; }
    public DateTime created_at { get; set; }
    public DateTime? updated_at { get; set; }
}

