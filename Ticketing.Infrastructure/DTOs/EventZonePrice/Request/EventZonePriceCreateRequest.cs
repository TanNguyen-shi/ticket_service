using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.EventZonePrice.Request;

public class EventZonePriceCreateRequest : IValidatableObject
{
    [Required(ErrorMessage = "event_zone_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "event_zone_id phải lớn hơn 0")]
    public long event_zone_id { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "price phải lớn hơn hoặc bằng 0")]
    public decimal price { get; set; }

    [Required(ErrorMessage = "currency là bắt buộc")]
    [StringLength(10, ErrorMessage = "currency phải có tối đa 10 ký tự")]
    public string currency { get; set; } = "VND";

    public DateTime? start_time { get; set; }
    public DateTime? end_time { get; set; }

    public bool is_active { get; set; } = true;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (start_time.HasValue && end_time.HasValue && end_time < start_time)
        {
            yield return new ValidationResult(
                "end_time phải lớn hơn hoặc bằng start_time",
                new[] { nameof(end_time), nameof(start_time) });
        }
    }
}

