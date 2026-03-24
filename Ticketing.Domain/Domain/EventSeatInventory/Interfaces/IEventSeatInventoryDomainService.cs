using Ticketing.Application.Model.DTOs;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.EventSeatInventory.Request;
using Ticketing.Infrastructure.DTOs.EventSeatInventory.Response;
using Ticketing.Infrastructure.Entities.EventSeatInventory.Response;

namespace Ticketing.Domain.Domain.EventSeatInventory.Interfaces;

public interface IEventSeatInventoryDomainService
{
    Task<ResponseMessage<int>> InsertAsync(EventSeatInventoryEntity entity, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> UpdateAsync(EventSeatInventoryEntity entity, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> DeleteAsync(EventSeatInventoryEntity entity, CancellationToken cancellationToken = default);

    Task<ResponseMessage<EventSeatInventoryDetailDto?>> GetByIdAsync(
        EventSeatInventoryGetByIdRequest request,
        CancellationToken cancellationToken = default);

    Task<ResponseMessage<IEnumerable<EventSeatInventoryListDto>>> GetPagedListAsync(
        EventSeatInventoryGetPagedListRequest request,
        CancellationToken cancellationToken = default);
}

