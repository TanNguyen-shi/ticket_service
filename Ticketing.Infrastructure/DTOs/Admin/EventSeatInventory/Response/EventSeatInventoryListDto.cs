using Ticketing.Infrastructure.DTOs.Admin.EventSeatInventory.Response;

namespace Ticketing.Infrastructure.DTOs.EventSeatInventory.Response;

public class EventSeatInventoryListDto : EventSeatInventoryDto
{
    public long row_index { get; set; }
    public long row_total { get; set; }
}

