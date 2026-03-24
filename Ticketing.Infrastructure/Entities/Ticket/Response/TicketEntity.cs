namespace Ticketing.Infrastructure.Entities.Ticket.Response;

public class TicketEntity : BaseEntity
{
    public long ticket_id { get; set; }
    public string ticket_code { get; set; } = string.Empty;
    public long order_item_id { get; set; }
    public long event_id { get; set; }
    public long user_id { get; set; }
    public long seat_id { get; set; }
    public string? seat_label_snapshot { get; set; }
    public string? zone_name_snapshot { get; set; }
    public string? event_name_snapshot { get; set; }
    public string ticket_status { get; set; } = string.Empty;
    public DateTime? issued_at { get; set; }
    public DateTime? checked_in_at { get; set; }
}

