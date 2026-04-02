namespace Ticketing.Infrastructure.DTOs.Event.Request;

public class EventGetPagedListRequest : BaseFilterPaging
{
    public string? status { get; set; } = string.Empty;
    public long venue_id { get; set; } = -1;
    public int? is_featured { get; set; } = -1;
    public int? is_trending { get; set; } = -1;
}

