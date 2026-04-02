using Ticketing.Application.Model.DTOs;
using Ticketing.Infrastructure.DTOs.Client.Event.Request;
using Ticketing.Infrastructure.DTOs.Client.Event.Response;
using Ticketing.Infrastructure.DTOs.Event.Request;
using Ticketing.Infrastructure.DTOs.Event.Response;

namespace Ticketing.Application.UseCases.Event.Interfaces;

/// <summary>
/// Client-side Event Use Cases
/// Dùng cho Homepage, Explore page,...
/// </summary>
public interface IEventClientUseCases
{
    Task<ResponseMessage<EventClientDetailDto?>> GetDetailEvent(EventGetByIdRequest request, CancellationToken cancellationToken = default);
    Task<ResponseMessage<IEnumerable<EventClientListDto>>> GetFeaturedAsync(EventGetFeaturedClientRequest request, CancellationToken cancellationToken = default);

    Task<ResponseMessage<IEnumerable<EventClientListDto>>> GetTrendingAsync(EventGetTrendingClientRequest request, CancellationToken cancellationToken = default);

    Task<ResponseMessage<IEnumerable<EventClientListDto>>> GetUpcomingAsync(EventGetUpcomingClientRequest request, CancellationToken cancellationToken = default);

    Task<ResponseMessage<IEnumerable<EventClientListDto>>> SearchAsync(EventSearchClientRequest request, CancellationToken cancellationToken = default);
}