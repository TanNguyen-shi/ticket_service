using Ticketing.Application.Model.DTOs;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.EventZonePrice.Request;
using Ticketing.Infrastructure.DTOs.EventZonePrice.Response;

namespace Ticketing.Application.UseCases.EventZonePrice.Interfaces;

public interface IEventZonePriceUseCases
{
    Task<ResponseMessage<int>?> InsertAsync(EventZonePriceCreateRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> UpdateAsync(EventZonePriceUpdateRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> DeleteAsync(EventZonePriceDeleteRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<EventZonePriceDetailDto?>> GetByIdAsync(EventZonePriceGetByIdRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<IEnumerable<EventZonePriceListDto>>> GetPagedListAsync(EventZonePriceGetPagedListRequest request, long? userLogin, CancellationToken cancellationToken = default);
}

