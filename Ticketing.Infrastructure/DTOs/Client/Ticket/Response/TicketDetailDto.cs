namespace Ticketing.Infrastructure.DTOs.Client.Ticket.Response;

public class TicketDetailDto
{
    public long ticket_id { get; set; }
    public string ticket_code { get; set; } = string.Empty;
    public long order_item_id { get; set; }
    public long event_id { get; set; }
    public long customer_id { get; set; }
    public long seat_id { get; set; }
    public string event_name_snapshot { get; set; } = string.Empty;
    public string seat_label_snapshot { get; set; } = string.Empty;
    public string zone_name_snapshot { get; set; } = string.Empty;
    public string ticket_status { get; set; } = string.Empty;
    public DateTime? issued_at { get; set; }
    public DateTime? checked_in_at { get; set; }
    public long order_id { get; set; }
    public string order_code { get; set; } = string.Empty;
    public decimal total_amount { get; set; }
    public decimal discount_amount { get; set; }
    public decimal final_amount { get; set; }
    public decimal unit_price { get; set; }
    public DateTime? paid_at { get; set; }
    public DateTime created_at { get; set; }
}
