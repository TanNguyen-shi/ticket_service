namespace Ticketing.Infrastructure.DTOs.Ticket.Response;

public class TicketListDto : TicketDto
{
    public long row_index { get; set; }
    public long row_total { get; set; }
}

