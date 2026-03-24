using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.VenueSection.Request;

public class VenueSectionCreateRequest : IValidatableObject
{
    [Required(ErrorMessage = "venue_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "venue_id must be greater than 0")]
    public long venue_id { get; set; }

    [Required(ErrorMessage = "section_code is required")]
    [StringLength(50, ErrorMessage = "section_code must be less than or equal to 50 characters")]
    public string section_code { get; set; } = string.Empty;

    [Required(ErrorMessage = "section_name is required")]
    [StringLength(255, ErrorMessage = "section_name must be less than or equal to 255 characters")]
    public string section_name { get; set; } = string.Empty;

    public int display_order { get; set; }

    [Required(ErrorMessage = "status is required")]
    [RegularExpression("^(active|inactive)$", ErrorMessage = "status must be active or inactive")]
    public string status { get; set; } = "active";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(section_code))
        {
            yield return new ValidationResult(
                "section_code must not be empty",
                new[] { nameof(section_code) });
        }

        if (string.IsNullOrWhiteSpace(section_name))
        {
            yield return new ValidationResult(
                "section_name must not be empty",
                new[] { nameof(section_name) });
        }
    }
}

