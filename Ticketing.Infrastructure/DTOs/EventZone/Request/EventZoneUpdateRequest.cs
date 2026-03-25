using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.EventZone.Request;

public class EventZoneUpdateRequest : IValidatableObject
{
    [Required(ErrorMessage = "event_zone_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "event_zone_id phải lớn hơn 0")]
    public long event_zone_id { get; set; }

    [Required(ErrorMessage = "event_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "event_id phải lớn hơn 0")]
    public long event_id { get; set; }

    [Required(ErrorMessage = "zone_code là bắt buộc")]
    [StringLength(50, ErrorMessage = "zone_code phải có tối đa 50 ký tự")]
    public string zone_code { get; set; } = string.Empty;

    [Required(ErrorMessage = "zone_name là bắt buộc")]
    [StringLength(255, ErrorMessage = "zone_name phải có tối đa 255 ký tự")]
    public string zone_name { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "color_hex phải có tối đa 20 ký tự")]
    public string? color_hex { get; set; }

    [StringLength(500, ErrorMessage = "description phải có tối đa 500 ký tự")]
    public string? description { get; set; }

    public int display_order { get; set; }

    [Required(ErrorMessage = "status là bắt buộc")]
    [StringLength(30, ErrorMessage = "status phải có tối đa 30 ký tự")]
    public string status { get; set; } = "active";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (event_zone_id <= 0)
        {
            yield return new ValidationResult(
                "event_zone_id phải lớn hơn 0",
                new[] { nameof(event_zone_id) });
        }

        if (string.IsNullOrWhiteSpace(zone_code))
        {
            yield return new ValidationResult(
                "zone_code không được để trống",
                new[] { nameof(zone_code) });
        }

        if (string.IsNullOrWhiteSpace(zone_name))
        {
            yield return new ValidationResult(
                "zone_name không được để trống",
                new[] { nameof(zone_name) });
        }
    }
}

