using Ticketing.Application.Model.DTOs;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.Admin.Event.Request;
using Ticketing.Infrastructure.DTOs.Client.Event.Request;
using Ticketing.Infrastructure.DTOs.Client.Event.Response;
using Ticketing.Infrastructure.DTOs.Event.Request;
using Ticketing.Infrastructure.DTOs.Event.Response;
using Ticketing.Infrastructure.Entities.Event.Response;

namespace Ticketing.Domain.Domain.Event.Interfaces;

public interface IEventDomainService
{
    Task<ResponseMessage<int>> InsertAsync(EventEntity entity, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> UpdateAsync(EventEntity entity, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> DeleteAsync(EventEntity entity, CancellationToken cancellationToken = default);

    Task<ResponseMessage<EventDetailDto?>> GetByIdAsync(EventGetByIdRequest request, CancellationToken cancellationToken = default);

    Task<ResponseMessage<IEnumerable<EventListDto>>> GetPagedListAsync(EventGetPagedListRequest request, CancellationToken cancellationToken = default);

    // Client-side methods
    Task<ResponseMessage<IEnumerable<EventClientListDto>>> GetFeaturedAsync(EventGetFeaturedClientRequest request, CancellationToken cancellationToken = default);

    Task<ResponseMessage<IEnumerable<EventClientListDto>>> GetTrendingAsync(EventGetTrendingClientRequest request, CancellationToken cancellationToken = default);

    Task<ResponseMessage<IEnumerable<EventClientListDto>>> GetUpcomingAsync(EventGetUpcomingClientRequest request, CancellationToken cancellationToken = default);

    Task<ResponseMessage<IEnumerable<EventClientListDto>>> SearchAsync(EventSearchClientRequest request, CancellationToken cancellationToken = default);
    Task<ResponseMessage<EventClientDetailDto?>> GetEventDetail(EventGetByIdRequest request, CancellationToken cancellationToken = default);
}