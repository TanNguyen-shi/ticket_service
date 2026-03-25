using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.VenueSeat.Request;

public class VenueSeatUpdateRequest : IValidatableObject
{
    [Required(ErrorMessage = "seat_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "seat_id phải lớn hơn 0")]
    public long seat_id { get; set; }

    [Required(ErrorMessage = "venue_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "venue_id phải lớn hơn 0")]
    public long venue_id { get; set; }

    [Required(ErrorMessage = "section_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "section_id phải lớn hơn 0")]
    public long section_id { get; set; }

    [Required(ErrorMessage = "seat_code là bắt buộc")]
    [StringLength(50, ErrorMessage = "seat_code phải có tối đa 50 ký tự")]
    public string seat_code { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "row_label phải có tối đa 50 ký tự")]
    public string? row_label { get; set; }

    [StringLength(50, ErrorMessage = "seat_number phải có tối đa 50 ký tự")]
    public string? seat_number { get; set; }

    [StringLength(100, ErrorMessage = "seat_label phải có tối đa 100 ký tự")]
    public string? seat_label { get; set; }

    public decimal? x_pos { get; set; }
    public decimal? y_pos { get; set; }

    [StringLength(30, ErrorMessage = "seat_type phải có tối đa 30 ký tự")]
    public string seat_type { get; set; } = "seat";

    [StringLength(30, ErrorMessage = "status phải có tối đa 30 ký tự")]
    public string status { get; set; } = "active";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (seat_id <= 0)
        {
            yield return new ValidationResult(
                "seat_id phải lớn hơn 0",
                new[] { nameof(seat_id) });
        }

        if (string.IsNullOrWhiteSpace(seat_code))
        {
            yield return new ValidationResult(
                "seat_code không được để trống",
                new[] { nameof(seat_code) });
        }
    }
}

