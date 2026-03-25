using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.Ticket.Request;

public class TicketDeleteRequest
{
    [Required(ErrorMessage = "ticket_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "ticket_id phải lớn hơn 0")]
    public long ticket_id { get; set; }
}

