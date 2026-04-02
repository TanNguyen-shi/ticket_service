using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.EventSeatInventory.Request;

public class EventSeatInventoryGetByIdRequest
{
    [Required(ErrorMessage = "event_seat_inventory_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "event_seat_inventory_id phải lớn hơn 0")]
    public long event_seat_inventory_id { get; set; }
}

