namespace Ticketing.Infrastructure.DTOs.TicketOrder.Response;

public class TicketOrderListDto : TicketOrderDto
{
    public long row_index { get; set; }
    public long row_total { get; set; }
}

