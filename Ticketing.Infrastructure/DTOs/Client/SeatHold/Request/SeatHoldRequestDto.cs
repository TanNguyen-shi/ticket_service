namespace Ticketing.Infrastructure.DTOs.Client.SeatHold.Request;

/// <summary>
/// Request DTO cho SeatHold Insert/Update
/// </summary>
public class SeatHoldInsertDto
{
    public string hold_code { get; set; } = string.Empty;
    public long event_id { get; set; }
    public long user_id { get; set; }
    public string status { get; set; } = "active";
    public DateTime hold_started_at { get; set; }
    public DateTime hold_expires_at { get; set; }
    public DateTime? released_at { get; set; }
    public string? release_reason { get; set; }
}

/// <summary>
/// Request DTO cho SeatHold Update
/// </summary>
public class SeatHoldUpdateDto
{
    public long hold_id { get; set; }
    public string hold_code { get; set; } = string.Empty;
    public long event_id { get; set; }
    public long user_id { get; set; }
    public string status { get; set; } = string.Empty;
    public DateTime hold_started_at { get; set; }
    public DateTime hold_expires_at { get; set; }
    public DateTime? released_at { get; set; }
    public string? release_reason { get; set; }
}

/// <summary>
/// Request DTO cho SeatHold Check
/// </summary>
public class SeatHoldCheckDto
{
    public long hold_id { get; set; } = 0;
    public string hold_code { get; set; } = string.Empty;
}

/// <summary>
/// Request DTO cho SeatHold Delete
/// </summary>
public class SeatHoldDeleteDto
{
    public long hold_id { get; set; }
}

/// <summary>
/// Request DTO cho SeatHold GetById
/// </summary>
public class SeatHoldGetByIdDto
{
    public long hold_id { get; set; }
}

/// <summary>
/// Request DTO cho SeatHold GetPagedList
/// </summary>
public class SeatHoldGetPagedListDto
{
    public int pagesize { get; set; } = 20;
    public int offset { get; set; } = 0;
    public string keysearch { get; set; } = string.Empty;
    public long event_id { get; set; } = -1;
    public long user_id { get; set; } = -1;
    public string status { get; set; } = string.Empty;
}

