namespace Ticketing.Infrastructure.DTOs.Ticket.Request;

public class TicketGetPagedListRequest : BaseFilterPaging
{
    public long event_id { get; set; } = -1;
    public long customer_id { get; set; } = -1;
    public string? ticket_status { get; set; } = string.Empty;
}

