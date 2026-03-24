using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.EventSeatInventory.Request;
using Ticketing.Infrastructure.DTOs.EventSeatInventory.Response;

namespace Ticketing.Application.UseCases.EventSeatInventory.Interfaces;

public interface IEventSeatInventoryUseCases
{
    Task<ResponseMessage<int>?> InsertAsync(EventSeatInventoryCreateRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> UpdateAsync(EventSeatInventoryUpdateRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> DeleteAsync(EventSeatInventoryDeleteRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<EventSeatInventoryDetailDto?>> GetByIdAsync(EventSeatInventoryGetByIdRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<IEnumerable<EventSeatInventoryListDto>>> GetPagedListAsync(EventSeatInventoryGetPagedListRequest request, long? userLogin, CancellationToken cancellationToken = default);
}

