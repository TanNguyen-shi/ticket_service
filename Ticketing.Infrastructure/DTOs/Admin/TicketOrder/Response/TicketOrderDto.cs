namespace Ticketing.Infrastructure.DTOs.TicketOrder.Response;

public class TicketOrderDto
{
    public long order_id { get; set; }
    public string order_code { get; set; } = string.Empty;
    public long event_id { get; set; }
    public string? event_code { get; set; }
    public string? event_name { get; set; }
    public long user_id { get; set; }
    public string? username { get; set; }
    public string? user_fullname { get; set; }
    public long? hold_id { get; set; }
    public string? hold_code { get; set; }
    public decimal total_amount { get; set; }
    public decimal discount_amount { get; set; }
    public decimal final_amount { get; set; }
    public string order_status { get; set; } = string.Empty;
    public DateTime? expired_at { get; set; }
    public DateTime? paid_at { get; set; }
    public DateTime created_at { get; set; }
    public DateTime? updated_at { get; set; }
}

