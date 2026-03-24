using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.Event.Request;

public class EventUpdateRequest : IValidatableObject
{
    [Required(ErrorMessage = "event_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "event_id must be greater than 0")]
    public long event_id { get; set; }

    [Required(ErrorMessage = "event_code is required")]
    [StringLength(50, ErrorMessage = "event_code must be less than or equal to 50 characters")]
    public string event_code { get; set; } = string.Empty;

    [Required(ErrorMessage = "event_name is required")]
    [StringLength(255, ErrorMessage = "event_name must be less than or equal to 255 characters")]
    public string event_name { get; set; } = string.Empty;

    public string? description { get; set; }

    [Required(ErrorMessage = "venue_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "venue_id must be greater than 0")]
    public long venue_id { get; set; }

    [StringLength(500, ErrorMessage = "banner_url must be less than or equal to 500 characters")]
    public string? banner_url { get; set; }

    [Required(ErrorMessage = "start_time is required")]
    public DateTime start_time { get; set; }

    [Required(ErrorMessage = "end_time is required")]
    public DateTime end_time { get; set; }

    public DateTime? sale_start_time { get; set; }
    public DateTime? sale_end_time { get; set; }

    [Required(ErrorMessage = "status is required")]
    [StringLength(30, ErrorMessage = "status must be less than or equal to 30 characters")]
    public string status { get; set; } = "draft";

    public DateTime? published_at { get; set; }
    public DateTime? on_sale_at { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (event_id <= 0)
        {
            yield return new ValidationResult(
                "event_id must be greater than 0",
                new[] { nameof(event_id) });
        }

        if (end_time < start_time)
        {
            yield return new ValidationResult(
                "end_time must be greater than or equal to start_time",
                new[] { nameof(end_time), nameof(start_time) });
        }

        if (sale_start_time.HasValue && sale_end_time.HasValue && sale_end_time < sale_start_time)
        {
            yield return new ValidationResult(
                "sale_end_time must be greater than or equal to sale_start_time",
                new[] { nameof(sale_end_time), nameof(sale_start_time) });
        }
    }
}

