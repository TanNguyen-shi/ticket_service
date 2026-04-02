using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.Venue.Request;

public class VenueGetPagedListRequest : BaseFilterPaging
{
    [RegularExpression("^(|active|inactive)$", ErrorMessage = "status phải để trống, active hoặc inactive")]
    public string? status { get; set; } = "active";

    public string? city { get; set; } = "";
}