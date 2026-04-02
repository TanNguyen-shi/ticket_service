using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.TicketOrderItem.Request;

public class TicketOrderItemCreateRequest
{
    [Required(ErrorMessage = "order_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "order_id phải lớn hơn 0")]
    public long order_id { get; set; }

    [Required(ErrorMessage = "event_seat_inventory_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "event_seat_inventory_id phải lớn hơn 0")]
    public long event_seat_inventory_id { get; set; }

    [Required(ErrorMessage = "seat_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "seat_id phải lớn hơn 0")]
    public long seat_id { get; set; }

    [Required(ErrorMessage = "zone_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "zone_id phải lớn hơn 0")]
    public long zone_id { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "unit_price phải lớn hơn hoặc bằng 0")]
    public decimal unit_price { get; set; }

    [StringLength(100, ErrorMessage = "seat_label_snapshot phải có tối đa 100 ký tự")]
    public string? seat_label_snapshot { get; set; }

    [StringLength(255, ErrorMessage = "zone_name_snapshot phải có tối đa 255 ký tự")]
    public string? zone_name_snapshot { get; set; }

    [Required(ErrorMessage = "item_status là bắt buộc")]
    [StringLength(50, ErrorMessage = "item_status phải có tối đa 50 ký tự")]
    public string item_status { get; set; } = "pending";
}

