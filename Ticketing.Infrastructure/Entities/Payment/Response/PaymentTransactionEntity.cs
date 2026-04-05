namespace Ticketing.Infrastructure.Entities.Payment.Response;

public class PaymentTransactionEntity : BaseEntity
{
    public long payment_id { get; set; }
    public long order_id { get; set; }
    public string payment_provider { get; set; } = string.Empty; // vnpay, momo, mock
    public string payment_ref { get; set; } = string.Empty;
    public string? provider_transaction_ref { get; set; }
    public decimal amount { get; set; }
    public string payment_status { get; set; } = string.Empty; // initiated, pending, success, failed, cancelled
    public DateTime requested_at { get; set; }
    public DateTime? confirmed_at { get; set; }
    public string? raw_request_payload { get; set; }
    public string? raw_callback_payload { get; set; }
}

