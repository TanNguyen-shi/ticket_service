using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.TicketOrder.Request;

public class TicketOrderCreateRequest
{
    [Required(ErrorMessage = "order_code là bắt buộc")]
    [StringLength(50, ErrorMessage = "order_code phải có tối đa 50 ký tự")]
    public string order_code { get; set; } = string.Empty;

    [Required(ErrorMessage = "event_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "event_id phải lớn hơn 0")]
    public long event_id { get; set; }

    [Required(ErrorMessage = "customer_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "customer_id phải lớn hơn 0")]
    public long customer_id { get; set; }

    public long? hold_id { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "total_amount phải lớn hơn hoặc bằng 0")]
    public decimal total_amount { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "discount_amount phải lớn hơn hoặc bằng 0")]
    public decimal discount_amount { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "final_amount phải lớn hơn hoặc bằng 0")]
    public decimal final_amount { get; set; }

    [Required(ErrorMessage = "order_status là bắt buộc")]
    [StringLength(50, ErrorMessage = "order_status phải có tối đa 50 ký tự")]
    public string order_status { get; set; } = "pending_payment";

    public DateTime? expired_at { get; set; }
    public DateTime? paid_at { get; set; }
}

