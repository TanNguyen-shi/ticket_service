namespace Ticketing.Infrastructure.DTOs.Ticket.Response;

public class TicketDto
{
    public long ticket_id { get; set; }
    public string ticket_code { get; set; } = string.Empty;
    public long order_item_id { get; set; }
    public long? order_id { get; set; }
    public string? order_code { get; set; }
    public long event_id { get; set; }
    public string? event_code { get; set; }
    public string? event_name { get; set; }
    public long customer_id { get; set; }
    public string? customer_email { get; set; }
    public string? customer_fullname { get; set; }
    public long seat_id { get; set; }
    public string? seat_code { get; set; }
    public string? seat_label { get; set; }
    public string? seat_label_snapshot { get; set; }
    public string? zone_name_snapshot { get; set; }
    public string? event_name_snapshot { get; set; }
    public string ticket_status { get; set; } = string.Empty;
    public DateTime? issued_at { get; set; }
    public DateTime? checked_in_at { get; set; }
}

