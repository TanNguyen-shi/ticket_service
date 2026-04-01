using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.Event.Request;

public class EventUpdateRequest : IValidatableObject
{
    [Required(ErrorMessage = "event_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "event_id phải lớn hơn 0")]
    public long event_id { get; set; }

    [Required(ErrorMessage = "event_code là bắt buộc")]
    [StringLength(50, ErrorMessage = "event_code phải có tối đa 50 ký tự")]
    public string event_code { get; set; } = string.Empty;

    [Required(ErrorMessage = "event_name là bắt buộc")]
    [StringLength(255, ErrorMessage = "event_name phải có tối đa 255 ký tự")]
    public string event_name { get; set; } = string.Empty;

    public string? description { get; set; }

    [Required(ErrorMessage = "venue_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "venue_id phải lớn hơn 0")]
    public long venue_id { get; set; }

    [StringLength(500, ErrorMessage = "banner_url phải có tối đa 500 ký tự")]
    public string? banner_url { get; set; }

    [Required(ErrorMessage = "start_time là bắt buộc")]
    public DateTime start_time { get; set; }

    [Required(ErrorMessage = "end_time là bắt buộc")]
    public DateTime end_time { get; set; }

    public DateTime? sale_start_time { get; set; }
    public DateTime? sale_end_time { get; set; }

    [Required(ErrorMessage = "status là bắt buộc")]
    [StringLength(30, ErrorMessage = "status phải có tối đa 30 ký tự")]
    public string status { get; set; } = "draft";

    public DateTime? published_at { get; set; }
    public DateTime? on_sale_at { get; set; }

    public bool is_featured { get; set; } = false;
    public bool is_trending { get; set; } = false;

    [Range(0, int.MaxValue, ErrorMessage = "display_order phải lớn hơn hoặc bằng 0")]
    public int display_order { get; set; } = 0;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (event_id <= 0)
        {
            yield return new ValidationResult(
                "event_id phải lớn hơn 0",
                new[] { nameof(event_id) });
        }

        if (end_time < start_time)
        {
            yield return new ValidationResult(
                "end_time phải lớn hơn hoặc bằng start_time",
                new[] { nameof(end_time), nameof(start_time) });
        }

        if (sale_start_time.HasValue && sale_end_time.HasValue && sale_end_time < sale_start_time)
        {
            yield return new ValidationResult(
                "sale_end_time phải lớn hơn hoặc bằng sale_start_time",
                new[] { nameof(sale_end_time), nameof(sale_start_time) });
        }
    }
}
