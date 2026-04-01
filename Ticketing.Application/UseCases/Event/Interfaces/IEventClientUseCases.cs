using Ticketing.Application.Model.DTOs;
using Ticketing.Infrastructure.DTOs.Event.Request;
using Ticketing.Infrastructure.DTOs.Event.Response;

namespace Ticketing.Application.UseCases.Event.Interfaces;

/// <summary>
/// Client-side Event Use Cases
/// Dùng cho Homepage, Explore page,...
/// </summary>
public interface IEventClientUseCases
{
    Task<ResponseMessage<IEnumerable<EventClientDto>>> GetFeaturedAsync(EventGetFeaturedClientRequest request, CancellationToken cancellationToken = default);

    Task<ResponseMessage<IEnumerable<EventClientDto>>> GetTrendingAsync(EventGetTrendingClientRequest request, CancellationToken cancellationToken = default);

    Task<ResponseMessage<IEnumerable<EventClientDto>>> GetUpcomingAsync(EventGetUpcomingClientRequest request, CancellationToken cancellationToken = default);

    Task<ResponseMessage<IEnumerable<EventClientDto>>> SearchAsync(EventSearchClientRequest request, CancellationToken cancellationToken = default);
}

