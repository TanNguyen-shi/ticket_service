namespace Ticketing.Infrastructure.Entities.TicketOrder.Response;

public class TicketOrderEntity : BaseEntity
{
    public long order_id { get; set; }
    public string order_code { get; set; } = string.Empty;
    public long event_id { get; set; }
    public long customer_id { get; set; }
    public long? hold_id { get; set; }
    public decimal total_amount { get; set; }
    public decimal discount_amount { get; set; }
    public decimal final_amount { get; set; }
    public string order_status { get; set; } = string.Empty;
    public DateTime? expired_at { get; set; }
    public DateTime? paid_at { get; set; }
}

