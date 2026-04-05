namespace Ticketing.Infrastructure.DTOs.Client.Event.Response;

public class EventSeatStatusDto
{
    public long venue_seat_id { get; set; }
    public string status { get; set; } = string.Empty; // 'available', 'booked', 'reserved', 'blocked'
    public long? order_id { get; set; }
}

