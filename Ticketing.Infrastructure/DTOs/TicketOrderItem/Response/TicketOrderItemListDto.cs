namespace Ticketing.Infrastructure.DTOs.TicketOrderItem.Response;

public class TicketOrderItemListDto : TicketOrderItemDto
{
    public long row_index { get; set; }
    public long row_total { get; set; }
}

