namespace Ticketing.Infrastructure.DTOs.Client.SeatHold.Response;

/// <summary>
/// Response DTO cho SeatHoldItem GetById
/// </summary>
public class SeatHoldItemResponseDto
{
    public long hold_item_id { get; set; }
    public long hold_id { get; set; }
    public string? hold_code { get; set; }
    public long event_id { get; set; }
    public string? event_code { get; set; }
    public string? event_name { get; set; }

    public long event_seat_inventory_id { get; set; }
    public long seat_id { get; set; }
    public string? seat_code { get; set; }
    public string? row_label { get; set; }
    public string? seat_number { get; set; }
    public string? seat_label { get; set; }

    public long zone_id { get; set; }
    public string? zone_code { get; set; }
    public string? zone_name { get; set; }

    public decimal price_at_hold { get; set; }
    public string? seat_label_snapshot { get; set; }
    public string? zone_name_snapshot { get; set; }
    public string status { get; set; } = string.Empty;
    public DateTime created_at { get; set; }
}

/// <summary>
/// Response DTO cho SeatHoldItem GetPagedList
/// </summary>
public class SeatHoldItemPagedDto
{
    public int row_index { get; set; }
    public int row_total { get; set; }

    public long hold_item_id { get; set; }
    public long hold_id { get; set; }
    public string? hold_code { get; set; }
    public long event_id { get; set; }
    public string? event_code { get; set; }
    public string? event_name { get; set; }

    public long event_seat_inventory_id { get; set; }
    public long seat_id { get; set; }
    public string? seat_code { get; set; }
    public string? row_label { get; set; }
    public string? seat_number { get; set; }
    public string? seat_label { get; set; }

    public long zone_id { get; set; }
    public string? zone_code { get; set; }
    public string? zone_name { get; set; }

    public decimal price_at_hold { get; set; }
    public string? seat_label_snapshot { get; set; }
    public string? zone_name_snapshot { get; set; }
    public string status { get; set; } = string.Empty;
    public DateTime created_at { get; set; }
}

