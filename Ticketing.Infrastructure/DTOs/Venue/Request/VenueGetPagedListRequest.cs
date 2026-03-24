using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.Venue.Request;

public class VenueGetPagedListRequest : BaseFilterPaging
{
    [RegularExpression("^(|active|inactive)$", ErrorMessage = "status must be empty, active or inactive")]
    public string? status { get; set; } = "active";

    public string? city { get; set; } = "";
}