using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.VenueSeat.Request;

public class VenueSeatGetByIdRequest
{
    [Required(ErrorMessage = "seat_id is required")]
    [Range(1, long.MaxValue, ErrorMessage = "seat_id must be greater than 0")]
    public long seat_id { get; set; }
}

