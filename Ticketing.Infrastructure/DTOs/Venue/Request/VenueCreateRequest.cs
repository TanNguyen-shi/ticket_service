using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.Venue.Request;

public class VenueCreateRequest : IValidatableObject
{
    [Required(ErrorMessage = "venue_code is required")]
    [StringLength(50, ErrorMessage = "venue_code must be less than or equal to 50 characters")]
    public string venue_code { get; set; } = string.Empty;

    [Required(ErrorMessage = "venue_name is required")]
    [StringLength(255, ErrorMessage = "venue_name must be less than or equal to 255 characters")]
    public string venue_name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "address_line must be less than or equal to 500 characters")]
    public string? address_line { get; set; }

    [StringLength(100, ErrorMessage = "city must be less than or equal to 100 characters")]
    public string? city { get; set; }

    [StringLength(100, ErrorMessage = "country must be less than or equal to 100 characters")]
    public string? country { get; set; }

    [Required(ErrorMessage = "status is required")]
    [RegularExpression("^(active|inactive)$", ErrorMessage = "status must be active or inactive")]
    public string status { get; set; } = "active";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(venue_code))
        {
            yield return new ValidationResult(
                "venue_code must not be empty",
                new[] { nameof(venue_code) });
        }

        if (string.IsNullOrWhiteSpace(venue_name))
        {
            yield return new ValidationResult(
                "venue_name must not be empty",
                new[] { nameof(venue_name) });
        }
    }
}