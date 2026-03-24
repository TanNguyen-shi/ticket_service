using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.EventZone.Request;

public class EventZoneUpdateRequest : IValidatableObject
{
    [Required(ErrorMessage = "event_zone_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "event_zone_id must be greater than 0")]
    public long event_zone_id { get; set; }

    [Required(ErrorMessage = "event_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "event_id must be greater than 0")]
    public long event_id { get; set; }

    [Required(ErrorMessage = "zone_code is required")]
    [StringLength(50, ErrorMessage = "zone_code must be less than or equal to 50 characters")]
    public string zone_code { get; set; } = string.Empty;

    [Required(ErrorMessage = "zone_name is required")]
    [StringLength(255, ErrorMessage = "zone_name must be less than or equal to 255 characters")]
    public string zone_name { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "color_hex must be less than or equal to 20 characters")]
    public string? color_hex { get; set; }

    [StringLength(500, ErrorMessage = "description must be less than or equal to 500 characters")]
    public string? description { get; set; }

    public int display_order { get; set; }

    [Required(ErrorMessage = "status is required")]
    [StringLength(30, ErrorMessage = "status must be less than or equal to 30 characters")]
    public string status { get; set; } = "active";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (event_zone_id <= 0)
        {
            yield return new ValidationResult(
                "event_zone_id must be greater than 0",
                new[] { nameof(event_zone_id) });
        }

        if (string.IsNullOrWhiteSpace(zone_code))
        {
            yield return new ValidationResult(
                "zone_code must not be empty",
                new[] { nameof(zone_code) });
        }

        if (string.IsNullOrWhiteSpace(zone_name))
        {
            yield return new ValidationResult(
                "zone_name must not be empty",
                new[] { nameof(zone_name) });
        }
    }
}

