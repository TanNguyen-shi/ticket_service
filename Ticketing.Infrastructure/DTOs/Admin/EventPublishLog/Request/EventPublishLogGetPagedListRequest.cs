namespace Ticketing.Infrastructure.DTOs.EventPublishLog.Request;

public class EventPublishLogGetPagedListRequest : BaseFilterPaging
{
    public long event_id { get; set; } = -1;
    public string? action { get; set; } = string.Empty;
    public string? new_status { get; set; } = string.Empty;
}

