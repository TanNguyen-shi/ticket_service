using System.ComponentModel.DataAnnotations;

namespace Ticketing.Infrastructure.Entities;

public enum TicketingTypeEnum
{
    [Display(Name = "Địa điểm")] Venue = 1,
    [Display(Name = "Ghế địa điểm")] VenueSeat = 2,
    [Display(Name = "Khu vực địa điểm")] VenueSection = 3,
    [Display(Name = "Sự kiện")] Event = 4,
    [Display(Name = "Lịch sử publish sự kiện")] EventPublishLog = 5,
    [Display(Name = "Tồn kho ghế sự kiện")] EventSeatInventory = 6,
    [Display(Name = "Vùng sự kiện")] EventZone = 7,
    [Display(Name = "Đơn hàng vé")] TicketOrder = 8,
    [Display(Name = "Chi tiết đơn hàng vé")] TicketOrderItem = 9,
    [Display(Name = "Vé")] Ticket = 10
}