namespace Ticketing.Infrastructure.Entities.VenueSeat.Response;

public class VenueSeatEntity : BaseEntity
{
    public long seat_id { get; set; }
    public long venue_id { get; set; }
    public long section_id { get; set; }
    public string seat_code { get; set; } = string.Empty;
    public string? row_label { get; set; }
    public string? seat_number { get; set; }
    public string? seat_label { get; set; }
    public decimal? x_pos { get; set; }
    public decimal? y_pos { get; set; }
    public string seat_type { get; set; } = "seat";
    public string status { get; set; } = "active";
}

