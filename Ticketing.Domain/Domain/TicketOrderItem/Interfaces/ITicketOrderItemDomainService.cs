using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.TicketOrderItem.Request;
using Ticketing.Infrastructure.DTOs.TicketOrderItem.Response;
using Ticketing.Infrastructure.Entities.TicketOrderItem.Response;

namespace Ticketing.Domain.Domain.TicketOrderItem.Interfaces;

public interface ITicketOrderItemDomainService
{
    Task<ResponseMessage<int>> InsertAsync(TicketOrderItemEntity entity, CancellationToken cancellationToken = default);
    Task<ResponseMessage<bool>> UpdateAsync(TicketOrderItemEntity entity, CancellationToken cancellationToken = default);
    Task<ResponseMessage<bool>> DeleteAsync(TicketOrderItemEntity entity, CancellationToken cancellationToken = default);
    Task<ResponseMessage<TicketOrderItemDetailDto?>> GetByIdAsync(TicketOrderItemGetByIdRequest request, CancellationToken cancellationToken = default);
    Task<ResponseMessage<IEnumerable<TicketOrderItemListDto>>> GetPagedListAsync(TicketOrderItemGetPagedListRequest request, CancellationToken cancellationToken = default);
}

