using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.VenueSection.Request;

public class VenueSectionCreateRequest : IValidatableObject
{
    [Required(ErrorMessage = "venue_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "venue_id phải lớn hơn 0")]
    public long venue_id { get; set; }

    [Required(ErrorMessage = "section_code là bắt buộc")]
    [StringLength(50, ErrorMessage = "section_code phải có tối đa 50 ký tự")]
    public string section_code { get; set; } = string.Empty;

    [Required(ErrorMessage = "section_name là bắt buộc")]
    [StringLength(255, ErrorMessage = "section_name phải có tối đa 255 ký tự")]
    public string section_name { get; set; } = string.Empty;

    public int display_order { get; set; }

    [Required(ErrorMessage = "status là bắt buộc")]
    [RegularExpression("^(active|inactive)$", ErrorMessage = "status phải là active hoặc inactive")]
    public string status { get; set; } = "active";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(section_code))
        {
            yield return new ValidationResult(
                "section_code không được để trống",
                new[] { nameof(section_code) });
        }

        if (string.IsNullOrWhiteSpace(section_name))
        {
            yield return new ValidationResult(
                "section_name không được để trống",
                new[] { nameof(section_name) });
        }
    }
}

