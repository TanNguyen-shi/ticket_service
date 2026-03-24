namespace Ticketing.Infrastructure.DTOs.TicketOrderItem.Request;

public class TicketOrderItemGetPagedListRequest : BaseFilterPaging
{
    public long order_id { get; set; } = -1;
    public long zone_id { get; set; } = -1;
    public string? item_status { get; set; } = string.Empty;
}

