namespace Ticketing.Infrastructure.DTOs.Event.Request;

public class EventGetPagedListRequest : BaseFilterPaging
{
    public string? status { get; set; } = string.Empty;
    public long venue_id { get; set; } = -1;
}

