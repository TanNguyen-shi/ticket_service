namespace Ticketing.Infrastructure.DTOs.Client.Ticket.Response;

public class TicketListItemDto
{
    public long ticket_id { get; set; }
    public string ticket_code { get; set; } = string.Empty;
    public long event_id { get; set; }
    public string event_name_snapshot { get; set; } = string.Empty;
    public string seat_label_snapshot { get; set; } = string.Empty;
    public string zone_name_snapshot { get; set; } = string.Empty;
    public string ticket_status { get; set; } = string.Empty;
    public DateTime? issued_at { get; set; }
    public DateTime? checked_in_at { get; set; }
    public long order_id { get; set; }
    public string order_code { get; set; } = string.Empty;
    public decimal final_amount { get; set; }
    public DateTime? paid_at { get; set; }
    public DateTime created_at { get; set; }
}
