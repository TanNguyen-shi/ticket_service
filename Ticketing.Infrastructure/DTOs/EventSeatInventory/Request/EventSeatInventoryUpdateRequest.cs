using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.EventSeatInventory.Request;

public class EventSeatInventoryUpdateRequest
{
    [Required(ErrorMessage = "event_seat_inventory_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "event_seat_inventory_id phải lớn hơn 0")]
    public long event_seat_inventory_id { get; set; }

    [Required(ErrorMessage = "event_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "event_id phải lớn hơn 0")]
    public long event_id { get; set; }

    [Required(ErrorMessage = "seat_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "seat_id phải lớn hơn 0")]
    public long seat_id { get; set; }

    [Required(ErrorMessage = "event_zone_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "event_zone_id phải lớn hơn 0")]
    public long event_zone_id { get; set; }

    [Required(ErrorMessage = "seat_status là bắt buộc")]
    [StringLength(30, ErrorMessage = "seat_status phải có tối đa 30 ký tự")]
    public string seat_status { get; set; } = "available";

    public long? current_hold_id { get; set; }
    public long? current_order_item_id { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "base_price phải lớn hơn hoặc bằng 0")]
    public decimal base_price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "version phải lớn hơn hoặc bằng 0")]
    public int version { get; set; }
}

