using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.EventPublishLog.Request;

public class EventPublishLogUpdateRequest : IValidatableObject
{
    [Required(ErrorMessage = "event_publish_log_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "event_publish_log_id must be greater than 0")]
    public long event_publish_log_id { get; set; }

    [Required(ErrorMessage = "event_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "event_id must be greater than 0")]
    public long event_id { get; set; }

    [Required(ErrorMessage = "action is required")]
    [StringLength(50, ErrorMessage = "action must be less than or equal to 50 characters")]
    public string action { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "old_status must be less than or equal to 50 characters")]
    public string? old_status { get; set; }

    [StringLength(50, ErrorMessage = "new_status must be less than or equal to 50 characters")]
    public string? new_status { get; set; }

    [StringLength(500, ErrorMessage = "note must be less than or equal to 500 characters")]
    public string? note { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (event_publish_log_id <= 0)
        {
            yield return new ValidationResult(
                "event_publish_log_id must be greater than 0",
                new[] { nameof(event_publish_log_id) });
        }

        if (string.IsNullOrWhiteSpace(action))
        {
            yield return new ValidationResult(
                "action must not be empty",
                new[] { nameof(action) });
        }
    }
}

