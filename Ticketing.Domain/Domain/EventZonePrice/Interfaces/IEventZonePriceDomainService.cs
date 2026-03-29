using Ticketing.Application.Model.DTOs;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.EventZonePrice.Request;
using Ticketing.Infrastructure.DTOs.EventZonePrice.Response;
using Ticketing.Infrastructure.Entities.EventZonePrice.Response;

namespace Ticketing.Domain.Domain.EventZonePrice.Interfaces;

public interface IEventZonePriceDomainService
{
    Task<ResponseMessage<int>> InsertAsync(EventZonePriceEntity entity, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> UpdateAsync(EventZonePriceEntity entity, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> DeleteAsync(EventZonePriceEntity entity, CancellationToken cancellationToken = default);

    Task<ResponseMessage<EventZonePriceDetailDto?>> GetByIdAsync(EventZonePriceGetByIdRequest request, CancellationToken cancellationToken = default);

    Task<ResponseMessage<IEnumerable<EventZonePriceListDto>>> GetPagedListAsync(EventZonePriceGetPagedListRequest request, CancellationToken cancellationToken = default);
}

