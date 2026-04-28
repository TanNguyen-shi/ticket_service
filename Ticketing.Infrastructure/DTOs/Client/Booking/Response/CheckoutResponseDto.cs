namespace Ticketing.Infrastructure.DTOs.Client.Booking.Response;

public class CheckoutResponseDto
{
    public long order_id { get; set; }
    public string order_code { get; set; } = string.Empty;
    public long event_id { get; set; }
    public string event_name { get; set; } = string.Empty;
    public decimal final_amount { get; set; }
    public DateTime paid_at { get; set; }
    public List<CheckoutTicketItemDto> tickets { get; set; } = [];
}

public class CheckoutTicketItemDto
{
    public long ticket_id { get; set; }
    public string ticket_code { get; set; } = string.Empty;
    public string seat_label { get; set; } = string.Empty;
    public string zone_name { get; set; } = string.Empty;
    public decimal price { get; set; }
}
