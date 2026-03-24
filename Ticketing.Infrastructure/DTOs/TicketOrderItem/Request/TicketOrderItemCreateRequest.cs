using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.TicketOrderItem.Request;

public class TicketOrderItemCreateRequest
{
    [Required(ErrorMessage = "order_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "order_id must be greater than 0")]
    public long order_id { get; set; }

    [Required(ErrorMessage = "event_seat_inventory_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "event_seat_inventory_id must be greater than 0")]
    public long event_seat_inventory_id { get; set; }

    [Required(ErrorMessage = "seat_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "seat_id must be greater than 0")]
    public long seat_id { get; set; }

    [Required(ErrorMessage = "zone_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "zone_id must be greater than 0")]
    public long zone_id { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "unit_price must be >= 0")]
    public decimal unit_price { get; set; }

    [StringLength(100, ErrorMessage = "seat_label_snapshot must be less than or equal to 100 characters")]
    public string? seat_label_snapshot { get; set; }

    [StringLength(255, ErrorMessage = "zone_name_snapshot must be less than or equal to 255 characters")]
    public string? zone_name_snapshot { get; set; }

    [Required(ErrorMessage = "item_status is required")]
    [StringLength(50, ErrorMessage = "item_status must be less than or equal to 50 characters")]
    public string item_status { get; set; } = "pending";
}

