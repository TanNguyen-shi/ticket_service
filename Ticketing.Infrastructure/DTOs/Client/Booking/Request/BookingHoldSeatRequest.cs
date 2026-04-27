namespace Ticketing.Infrastructure.DTOs.Client.Booking.Request;

public class BookingHoldSeatRequest
{
    public long event_id { get; set; }
    public List<long> seat_ids { get; set; } = new List<long>();
    public string idempotency_key { get; set; } = string.Empty;
}