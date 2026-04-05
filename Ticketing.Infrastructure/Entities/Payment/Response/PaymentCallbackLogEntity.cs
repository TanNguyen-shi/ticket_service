namespace Ticketing.Infrastructure.Entities.Payment.Response;

public class PaymentCallbackLogEntity : BaseEntity
{
    public long callback_log_id { get; set; }
    public long payment_id { get; set; }
    public string payment_provider { get; set; } = string.Empty;
    public string? external_transaction_ref { get; set; }
    public string? callback_signature { get; set; }
    public string? payload { get; set; }
    public bool signature_valid { get; set; }
    public string processed_status { get; set; } = string.Empty; // received, processed, ignored, failed
    public DateTime received_at { get; set; }
    public DateTime? processed_at { get; set; }
}

