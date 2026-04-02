namespace Ticketing.Infrastructure.DTOs.Event.Response;

public class EventListDto : EventDto
{
    public long row_index { get; set; }
    public long row_total { get; set; }
}

