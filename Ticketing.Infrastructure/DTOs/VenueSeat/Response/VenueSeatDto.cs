namespace Ticketing.Infrastructure.DTOs.VenueSeat.Response;

public class VenueSeatDto : AuditTable
{
    public long seat_id { get; set; }
    public long venue_id { get; set; }
    public string venue_code { get; set; } = string.Empty;
    public string venue_name { get; set; } = string.Empty;
    public long section_id { get; set; }
    public string section_code { get; set; } = string.Empty;
    public string section_name { get; set; } = string.Empty;
    public string seat_code { get; set; } = string.Empty;
    public string? row_label { get; set; }
    public string? seat_number { get; set; }
    public string? seat_label { get; set; }
    public decimal? x_pos { get; set; }
    public decimal? y_pos { get; set; }
    public string seat_type { get; set; } = string.Empty;
    public string status { get; set; } = string.Empty;
}

