using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.TicketOrderItem.Request;

public class TicketOrderItemDeleteRequest
{
    [Required(ErrorMessage = "order_item_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "order_item_id phải lớn hơn 0")]
    public long order_item_id { get; set; }
}

