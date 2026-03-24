using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.Ticket.Request;

public class TicketGetByIdRequest
{
    [Required(ErrorMessage = "ticket_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "ticket_id must be greater than 0")]
    public long ticket_id { get; set; }
}

