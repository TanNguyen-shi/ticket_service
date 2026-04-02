using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.Ticket.Request;

public class TicketCreateRequest
{
    [Required(ErrorMessage = "ticket_code là bắt buộc")]
    [StringLength(50, ErrorMessage = "ticket_code phải có tối đa 50 ký tự")]
    public string ticket_code { get; set; } = string.Empty;

    [Required(ErrorMessage = "order_item_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "order_item_id phải lớn hơn 0")]
    public long order_item_id { get; set; }

    [Required(ErrorMessage = "event_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "event_id phải lớn hơn 0")]
    public long event_id { get; set; }

    [Required(ErrorMessage = "user_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "user_id phải lớn hơn 0")]
    public long user_id { get; set; }

    [Required(ErrorMessage = "seat_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "seat_id phải lớn hơn 0")]
    public long seat_id { get; set; }

    public string? seat_label_snapshot { get; set; }
    public string? zone_name_snapshot { get; set; }
    public string? event_name_snapshot { get; set; }

    [Required(ErrorMessage = "ticket_status là bắt buộc")]
    [StringLength(50, ErrorMessage = "ticket_status phải có tối đa 50 ký tự")]
    public string ticket_status { get; set; } = "active";

    public DateTime? issued_at { get; set; }
    public DateTime? checked_in_at { get; set; }
}

