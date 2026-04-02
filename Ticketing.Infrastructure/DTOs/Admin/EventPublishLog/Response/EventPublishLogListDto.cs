namespace Ticketing.Infrastructure.DTOs.EventPublishLog.Response;

public class EventPublishLogListDto : EventPublishLogDto
{
    public long row_index { get; set; }
    public long row_total { get; set; }
}

