using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.EventPublishLog.Request;
using Ticketing.Infrastructure.DTOs.EventPublishLog.Response;
using Ticketing.Infrastructure.Entities.EventPublishLog.Response;

namespace Ticketing.Domain.Domain.EventPublishLog.Interfaces;

public interface IEventPublishLogDomainService
{
    Task<ResponseMessage<int>> InsertAsync(EventPublishLogEntity entity, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> UpdateAsync(EventPublishLogEntity entity, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> DeleteAsync(EventPublishLogEntity entity, CancellationToken cancellationToken = default);

    Task<ResponseMessage<EventPublishLogDetailDto?>> GetByIdAsync(
        EventPublishLogGetByIdRequest request,
        CancellationToken cancellationToken = default);

    Task<ResponseMessage<IEnumerable<EventPublishLogListDto>>> GetPagedListAsync(
        EventPublishLogGetPagedListRequest request,
        CancellationToken cancellationToken = default);
}

