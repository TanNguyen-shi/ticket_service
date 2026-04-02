namespace Ticketing.Infrastructure.DTOs.TicketOrder.Request;

public class TicketOrderGetPagedListRequest : BaseFilterPaging
{
    public long event_id { get; set; } = -1;
    public long user_id { get; set; } = -1;
    public string? order_status { get; set; } = string.Empty;
}

