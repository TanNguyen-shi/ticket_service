namespace Ticketing.Infrastructure.DTOs.Client.Booking.Response;

public class ReleaseHoldResponseDto
{
    public long hold_id { get; set; }
    public string release_reason { get; set; } = string.Empty;
    public DateTime released_at { get; set; }
}
