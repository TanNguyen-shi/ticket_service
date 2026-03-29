using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.DTOs.VenueSeat.Request;

public class VenueSeatDeleteRequest
{
    [Required(ErrorMessage = "seat_id là bắt buộc")]
    [Range(1, long.MaxValue, ErrorMessage = "seat_id phải lớn hơn 0")]
    public long seat_id { get; set; }
}

