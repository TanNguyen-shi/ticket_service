using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.VenueSeat.Request;

public class VenueSeatCreateRequest : IValidatableObject
{
    [Required(ErrorMessage = "venue_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "venue_id must be greater than 0")]
    public long venue_id { get; set; }

    [Required(ErrorMessage = "section_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "section_id must be greater than 0")]
    public long section_id { get; set; }

    [Required(ErrorMessage = "seat_code is required")]
    [StringLength(50, ErrorMessage = "seat_code must be less than or equal to 50 characters")]
    public string seat_code { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "row_label must be less than or equal to 50 characters")]
    public string? row_label { get; set; }

    [StringLength(50, ErrorMessage = "seat_number must be less than or equal to 50 characters")]
    public string? seat_number { get; set; }

    [StringLength(100, ErrorMessage = "seat_label must be less than or equal to 100 characters")]
    public string? seat_label { get; set; }

    public decimal? x_pos { get; set; }
    public decimal? y_pos { get; set; }

    [StringLength(30, ErrorMessage = "seat_type must be less than or equal to 30 characters")]
    public string seat_type { get; set; } = "seat";

    [StringLength(30, ErrorMessage = "status must be less than or equal to 30 characters")]
    public string status { get; set; } = "active";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(seat_code))
        {
            yield return new ValidationResult(
                "seat_code must not be empty",
                new[] { nameof(seat_code) });
        }
    }
}

