using Ticketing.Application.Model.DTOs;
using Ticketing.Infrastructure.DTOs;
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
    Task<ResponseMessage<IEnumerable<EventClientDto>>> GetFeaturedAsync(EventGetFeaturedClientRequest request, CancellationToken cancellationToken = default);

    Task<ResponseMessage<IEnumerable<EventClientDto>>> GetTrendingAsync(EventGetTrendingClientRequest request, CancellationToken cancellationToken = default);

    Task<ResponseMessage<IEnumerable<EventClientDto>>> GetUpcomingAsync(EventGetUpcomingClientRequest request, CancellationToken cancellationToken = default);

    Task<ResponseMessage<IEnumerable<EventClientDto>>> SearchAsync(EventSearchClientRequest request, CancellationToken cancellationToken = default);
}

