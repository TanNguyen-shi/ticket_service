using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.TicketOrder.Request;

public class TicketOrderGetByIdRequest
{
    [Required(ErrorMessage = "order_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "order_id must be greater than 0")]
    public long order_id { get; set; }
}

