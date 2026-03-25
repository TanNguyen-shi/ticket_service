using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.TicketOrder.Request;

public class TicketOrderGetByIdRequest
{
    [Required(ErrorMessage = "order_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "order_id phải lớn hơn 0")]
    public long order_id { get; set; }
}

