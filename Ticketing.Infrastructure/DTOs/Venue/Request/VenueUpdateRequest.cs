using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.Venue.Request;

public class VenueUpdateRequest : IValidatableObject
{
    [Required(ErrorMessage = "venue_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "venue_id phải lớn hơn 0")]
    public long venue_id { get; set; }

    [Required(ErrorMessage = "venue_code là bắt buộc")]
    [StringLength(50, ErrorMessage = "venue_code phải có tối đa 50 ký tự")]
    public string venue_code { get; set; } = string.Empty;

    [Required(ErrorMessage = "venue_name là bắt buộc")]
    [StringLength(255, ErrorMessage = "venue_name phải có tối đa 255 ký tự")]
    public string venue_name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "address_line phải có tối đa 500 ký tự")]
    public string? address_line { get; set; }

    [StringLength(100, ErrorMessage = "city phải có tối đa 100 ký tự")]
    public string? city { get; set; }

    [StringLength(100, ErrorMessage = "country phải có tối đa 100 ký tự")]
    public string? country { get; set; }

    [Required(ErrorMessage = "status là bắt buộc")]
    [RegularExpression("^(active|inactive)$", ErrorMessage = "status phải là active hoặc inactive")]
    public string status { get; set; } = "active";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (venue_id <= 0)
        {
            yield return new ValidationResult(
                "venue_id phải lớn hơn 0",
                new[] { nameof(venue_id) });
        }

        if (string.IsNullOrWhiteSpace(venue_code))
        {
            yield return new ValidationResult(
                "venue_code không được để trống",
                new[] { nameof(venue_code) });
        }

        if (string.IsNullOrWhiteSpace(venue_name))
        {
            yield return new ValidationResult(
                "venue_name không được để trống",
                new[] { nameof(venue_name) });
        }
    }
}