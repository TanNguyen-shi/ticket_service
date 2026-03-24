using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.EventSeatInventory.Request;

public class EventSeatInventoryDeleteRequest
{
    [Required(ErrorMessage = "event_seat_inventory_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "event_seat_inventory_id must be greater than 0")]
    public long event_seat_inventory_id { get; set; }
}

