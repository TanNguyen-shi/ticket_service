using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.EventZone.Request;
using Ticketing.Infrastructure.DTOs.EventZone.Response;
using Ticketing.Infrastructure.Entities.EventZone.Response;

namespace Ticketing.Domain.Domain.EventZone.Interfaces;

public interface IEventZoneDomainService
{
    Task<ResponseMessage<int>> InsertAsync(EventZoneEntity entity, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> UpdateAsync(EventZoneEntity entity, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> DeleteAsync(EventZoneEntity entity, CancellationToken cancellationToken = default);

    Task<ResponseMessage<EventZoneDetailDto?>> GetByIdAsync(EventZoneGetByIdRequest request, CancellationToken cancellationToken = default);

    Task<ResponseMessage<IEnumerable<EventZoneListDto>>> GetPagedListAsync(EventZoneGetPagedListRequest request, CancellationToken cancellationToken = default);
}

