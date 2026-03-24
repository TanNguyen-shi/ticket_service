using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.TicketOrderItem.Request;

public class TicketOrderItemGetByIdRequest
{
    [Required(ErrorMessage = "order_item_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "order_item_id must be greater than 0")]
    public long order_item_id { get; set; }
}

