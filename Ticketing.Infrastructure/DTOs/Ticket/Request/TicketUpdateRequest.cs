using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.Ticket.Request;

public class TicketUpdateRequest
{
    [Required(ErrorMessage = "ticket_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "ticket_id must be greater than 0")]
    public long ticket_id { get; set; }

    [Required(ErrorMessage = "ticket_code is required")]
    [StringLength(50, ErrorMessage = "ticket_code must be less than or equal to 50 characters")]
    public string ticket_code { get; set; } = string.Empty;

    [Required(ErrorMessage = "order_item_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "order_item_id must be greater than 0")]
    public long order_item_id { get; set; }

    [Required(ErrorMessage = "event_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "event_id must be greater than 0")]
    public long event_id { get; set; }

    [Required(ErrorMessage = "user_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "user_id must be greater than 0")]
    public long user_id { get; set; }

    [Required(ErrorMessage = "seat_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "seat_id must be greater than 0")]
    public long seat_id { get; set; }

    public string? seat_label_snapshot { get; set; }
    public string? zone_name_snapshot { get; set; }
    public string? event_name_snapshot { get; set; }

    [Required(ErrorMessage = "ticket_status is required")]
    [StringLength(50, ErrorMessage = "ticket_status must be less than or equal to 50 characters")]
    public string ticket_status { get; set; } = "active";

    public DateTime? issued_at { get; set; }
    public DateTime? checked_in_at { get; set; }
}

