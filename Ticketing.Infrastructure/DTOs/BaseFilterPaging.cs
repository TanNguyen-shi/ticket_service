namespace Ticketing.Infrastructure.DTOs;

public class BaseFilterPaging
{
    public int pagesize { get; set; } = 20;
    public int offset { get; set; } = 0;
    public string? keysearch { get; set; } = "";
}