using Ticketing.Application.Model.DTOs;
using Ticketing.Infrastructure.DTOs.Client.Booking.Request;
using Ticketing.Infrastructure.DTOs.Client.Booking.Response;

namespace Ticketing.Application.UseCases.Client.Booking.Interfaces;

public interface IBookingUseCases
{
    Task<ResponseMessage<BookingHoldSeatDto>> HoldSeat(BookingHoldSeatRequest request, long userId, CancellationToken cancellationToken = default);
    Task<ResponseMessage<CheckoutResponseDto>> CheckoutAsync(CheckoutRequest request, long customerId, CancellationToken cancellationToken = default);
}