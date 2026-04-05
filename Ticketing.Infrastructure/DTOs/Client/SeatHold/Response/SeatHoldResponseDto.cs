namespace Ticketing.Infrastructure.DTOs.Client.SeatHold.Response;

/// <summary>
/// Response DTO cho SeatHold GetById
/// </summary>
public class SeatHoldResponseDto
{
    public long hold_id { get; set; }
    public string hold_code { get; set; } = string.Empty;
    public long event_id { get; set; }
    public string? event_code { get; set; }
    public string? event_name { get; set; }
    public long user_id { get; set; }
    public string? username { get; set; }
    public string? full_name { get; set; }
    public string status { get; set; } = string.Empty;
    public DateTime hold_started_at { get; set; }
    public DateTime hold_expires_at { get; set; }
    public DateTime? released_at { get; set; }
    public string? release_reason { get; set; }
    public DateTime created_at { get; set; }
}

/// <summary>
/// Response DTO cho SeatHold GetPagedList
/// </summary>
public class SeatHoldPagedDto
{
    public int row_index { get; set; }
    public int row_total { get; set; }
    public long hold_id { get; set; }
    public string hold_code { get; set; } = string.Empty;
    public long event_id { get; set; }
    public string? event_code { get; set; }
    public string? event_name { get; set; }
    public long user_id { get; set; }
    public string? username { get; set; }
    public string? full_name { get; set; }
    public string status { get; set; } = string.Empty;
    public DateTime hold_started_at { get; set; }
    public DateTime hold_expires_at { get; set; }
    public DateTime? released_at { get; set; }
    public string? release_reason { get; set; }
    public DateTime created_at { get; set; }
}

