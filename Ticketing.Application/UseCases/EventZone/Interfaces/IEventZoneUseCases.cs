using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.EventZone.Request;
using Ticketing.Infrastructure.DTOs.EventZone.Response;

namespace Ticketing.Application.UseCases.EventZone.Interfaces;

public interface IEventZoneUseCases
{
    Task<ResponseMessage<int>?> InsertAsync(EventZoneCreateRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> UpdateAsync(EventZoneUpdateRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> DeleteAsync(EventZoneDeleteRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<EventZoneDetailDto?>> GetByIdAsync(EventZoneGetByIdRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<IEnumerable<EventZoneListDto>>> GetPagedListAsync(EventZoneGetPagedListRequest request, long? userLogin, CancellationToken cancellationToken = default);
}

