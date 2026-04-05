namespace Ticketing.Infrastructure.DTOs.Client.SeatHold.Request;

/// <summary>
/// Request DTO cho SeatHoldItem Insert/Update
/// </summary>
public class SeatHoldItemInsertDto
{
    public long hold_id { get; set; }
    public long event_seat_inventory_id { get; set; }
    public long seat_id { get; set; }
    public long zone_id { get; set; }
    public decimal price_at_hold { get; set; }
    public string? seat_label_snapshot { get; set; }
    public string? zone_name_snapshot { get; set; }
    public string status { get; set; } = "active";
}

/// <summary>
/// Request DTO cho SeatHoldItem Update
/// </summary>
public class SeatHoldItemUpdateDto
{
    public long hold_item_id { get; set; }
    public long hold_id { get; set; }
    public long event_seat_inventory_id { get; set; }
    public long seat_id { get; set; }
    public long zone_id { get; set; }
    public decimal price_at_hold { get; set; }
    public string? seat_label_snapshot { get; set; }
    public string? zone_name_snapshot { get; set; }
    public string status { get; set; } = string.Empty;
}

/// <summary>
/// Request DTO cho SeatHoldItem Check
/// </summary>
public class SeatHoldItemCheckDto
{
    public long hold_item_id { get; set; } = 0;
    public long hold_id { get; set; } = 0;
    public long event_seat_inventory_id { get; set; } = 0;
}

/// <summary>
/// Request DTO cho SeatHoldItem Delete
/// </summary>
public class SeatHoldItemDeleteDto
{
    public long hold_item_id { get; set; }
}

/// <summary>
/// Request DTO cho SeatHoldItem GetById
/// </summary>
public class SeatHoldItemGetByIdDto
{
    public long hold_item_id { get; set; }
}

/// <summary>
/// Request DTO cho SeatHoldItem GetPagedList
/// </summary>
public class SeatHoldItemGetPagedListDto
{
    public int pagesize { get; set; } = 20;
    public int offset { get; set; } = 0;
    public long hold_id { get; set; } = -1;
    public long event_seat_inventory_id { get; set; } = -1;
    public long zone_id { get; set; } = -1;
    public string status { get; set; } = string.Empty;
}

