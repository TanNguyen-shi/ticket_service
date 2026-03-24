using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.TicketOrder.Request;

public class TicketOrderUpdateRequest
{
    [Required(ErrorMessage = "order_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "order_id must be greater than 0")]
    public long order_id { get; set; }

    [Required(ErrorMessage = "order_code is required")]
    [StringLength(50, ErrorMessage = "order_code must be less than or equal to 50 characters")]
    public string order_code { get; set; } = string.Empty;

    [Required(ErrorMessage = "event_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "event_id must be greater than 0")]
    public long event_id { get; set; }

    [Required(ErrorMessage = "user_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "user_id must be greater than 0")]
    public long user_id { get; set; }

    public long? hold_id { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "total_amount must be >= 0")]
    public decimal total_amount { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "discount_amount must be >= 0")]
    public decimal discount_amount { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "final_amount must be >= 0")]
    public decimal final_amount { get; set; }

    [Required(ErrorMessage = "order_status is required")]
    [StringLength(50, ErrorMessage = "order_status must be less than or equal to 50 characters")]
    public string order_status { get; set; } = "pending_payment";

    public DateTime? expired_at { get; set; }
    public DateTime? paid_at { get; set; }
}

