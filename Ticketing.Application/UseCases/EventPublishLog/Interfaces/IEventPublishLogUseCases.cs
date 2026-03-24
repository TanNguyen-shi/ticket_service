using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.EventPublishLog.Request;
using Ticketing.Infrastructure.DTOs.EventPublishLog.Response;

namespace Ticketing.Application.UseCases.EventPublishLog.Interfaces;

public interface IEventPublishLogUseCases
{
    Task<ResponseMessage<int>?> InsertAsync(EventPublishLogCreateRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> UpdateAsync(EventPublishLogUpdateRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> DeleteAsync(EventPublishLogDeleteRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<EventPublishLogDetailDto?>> GetByIdAsync(EventPublishLogGetByIdRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<IEnumerable<EventPublishLogListDto>>> GetPagedListAsync(EventPublishLogGetPagedListRequest request, long? userLogin, CancellationToken cancellationToken = default);
}

