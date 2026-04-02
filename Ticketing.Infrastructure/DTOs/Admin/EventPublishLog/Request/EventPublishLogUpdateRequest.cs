using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.EventPublishLog.Request;

public class EventPublishLogUpdateRequest : IValidatableObject
{
    [Required(ErrorMessage = "event_publish_log_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "event_publish_log_id phải lớn hơn 0")]
    public long event_publish_log_id { get; set; }

    [Required(ErrorMessage = "event_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "event_id phải lớn hơn 0")]
    public long event_id { get; set; }

    [Required(ErrorMessage = "action là bắt buộc")]
    [StringLength(50, ErrorMessage = "action phải có tối đa 50 ký tự")]
    public string action { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "old_status phải có tối đa 50 ký tự")]
    public string? old_status { get; set; }

    [StringLength(50, ErrorMessage = "new_status phải có tối đa 50 ký tự")]
    public string? new_status { get; set; }

    [StringLength(500, ErrorMessage = "note phải có tối đa 500 ký tự")]
    public string? note { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (event_publish_log_id <= 0)
        {
            yield return new ValidationResult(
                "event_publish_log_id phải lớn hơn 0",
                new[] { nameof(event_publish_log_id) });
        }

        if (string.IsNullOrWhiteSpace(action))
        {
            yield return new ValidationResult(
                "action không được để trống",
                new[] { nameof(action) });
        }
    }
}

