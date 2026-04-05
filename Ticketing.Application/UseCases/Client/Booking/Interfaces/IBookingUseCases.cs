using Ticketing.Application.Model.DTOs;
using Ticketing.Infrastructure.DTOs.Admin.Event.Request;
using Ticketing.Infrastructure.DTOs.Client.Booking.Request;
using Ticketing.Infrastructure.DTOs.Client.Booking.Response;
using Ticketing.Infrastructure.DTOs.Client.Event.Request;
using Ticketing.Infrastructure.DTOs.Client.Event.Response;

namespace Ticketing.Application.UseCases.Client.Booking.Interfaces;

/// <summary>
/// Client-side Event Use Cases
/// Dùng cho Homepage, Explore page,...
/// </summary>
public interface IBookingUseCases
{
    Task<ResponseMessage<BookingHoldSeatDto>> HoldSeat(BookingHoldSeatRequest request, CancellationToken cancellationToken = default);
}