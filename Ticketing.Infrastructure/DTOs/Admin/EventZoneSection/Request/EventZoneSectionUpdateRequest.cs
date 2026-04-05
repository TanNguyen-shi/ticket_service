using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.EventZoneSection.Request;

public class EventZoneSectionUpdateRequest : IValidatableObject
{
    [Required(ErrorMessage = "event_zone_section_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "event_zone_section_id phải lớn hơn 0")]
    public long event_zone_section_id { get; set; }

    [Required(ErrorMessage = "event_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "event_id phải lớn hơn 0")]
    public long event_id { get; set; }

    [Required(ErrorMessage = "event_zone_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "event_zone_id phải lớn hơn 0")]
    public long event_zone_id { get; set; }

    [Required(ErrorMessage = "section_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "section_id phải lớn hơn 0")]
    public long section_id { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (event_zone_section_id <= 0)
        {
            yield return new ValidationResult(
                "event_zone_section_id phải lớn hơn 0",
                new[] { nameof(event_zone_section_id) });
        }

        if (event_id <= 0)
        {
            yield return new ValidationResult(
                "event_id phải lớn hơn 0",
                new[] { nameof(event_id) });
        }

        if (event_zone_id <= 0)
        {
            yield return new ValidationResult(
                "event_zone_id phải lớn hơn 0",
                new[] { nameof(event_zone_id) });
        }

        if (section_id <= 0)
        {
            yield return new ValidationResult(
                "section_id phải lớn hơn 0",
                new[] { nameof(section_id) });
        }
    }
}

