namespace Ticketing.Infrastructure.DTOs.Client.Booking.Response;

/// <summary>
/// Response DTO cho HoldSeat endpoint
/// Chứa thông tin phiên giữ chỗ và danh sách ghế được giữ
/// </summary>
public class BookingHoldSeatDto
{
    /// <summary>
    /// ID của phiên giữ chỗ
    /// </summary>
    public long? hold_id { get; set; }


    /// <summary>
    /// Event ID của sự kiện
    /// </summary>
    public long event_id { get; set; }

    /// <summary>
    /// Thời điểm bắt đầu giữ chỗ
    /// </summary>
    public DateTime hold_started_at { get; set; }

    /// <summary>
    /// Thời điểm hết hạn giữ chỗ (10 phút sau bắt đầu)
    /// Dùng cho Frontend countdown timer
    /// </summary>
    public DateTime hold_expires_at { get; set; }

    /// <summary>
    /// Danh sách ghế được giữ trong phiên này
    /// </summary>
    public List<HeldSeatItemDto> held_seats { get; set; } = new();
}

/// <summary>
/// Chi tiết mỗi ghế trong phiên giữ chỗ
/// </summary>
public class HeldSeatItemDto
{
    /// <summary>
    /// ID của event_seat_inventory
    /// </summary>
    public long event_seat_inventory_id { get; set; }

    /// <summary>
    /// ID của ghế
    /// </summary>
    public int seat_hold_item_id { get; set; }
}