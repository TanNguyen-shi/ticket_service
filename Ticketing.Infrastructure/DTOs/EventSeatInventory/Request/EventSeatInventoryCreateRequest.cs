using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.EventSeatInventory.Request;

public class EventSeatInventoryCreateRequest
{
    [Required(ErrorMessage = "event_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "event_id must be greater than 0")]
    public long event_id { get; set; }

    [Required(ErrorMessage = "seat_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "seat_id must be greater than 0")]
    public long seat_id { get; set; }

    [Required(ErrorMessage = "event_zone_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "event_zone_id must be greater than 0")]
    public long event_zone_id { get; set; }

    [Required(ErrorMessage = "seat_status is required")]
    [StringLength(30, ErrorMessage = "seat_status must be less than or equal to 30 characters")]
    public string seat_status { get; set; } = "available";

    public long? current_hold_id { get; set; }
    public long? current_order_item_id { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "base_price must be >= 0")]
    public decimal base_price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "version must be >= 0")]
    public int version { get; set; } = 1;
}

