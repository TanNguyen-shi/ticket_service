using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.TicketOrder.Request;
using Ticketing.Infrastructure.DTOs.TicketOrder.Response;
using Ticketing.Infrastructure.Entities.TicketOrder.Response;

namespace Ticketing.Domain.Domain.TicketOrder.Interfaces;

public interface ITicketOrderDomainService
{
    Task<ResponseMessage<int>> InsertAsync(TicketOrderEntity entity, CancellationToken cancellationToken = default);
    Task<ResponseMessage<bool>> UpdateAsync(TicketOrderEntity entity, CancellationToken cancellationToken = default);
    Task<ResponseMessage<bool>> DeleteAsync(TicketOrderEntity entity, CancellationToken cancellationToken = default);
    Task<ResponseMessage<TicketOrderDetailDto?>> GetByIdAsync(TicketOrderGetByIdRequest request, CancellationToken cancellationToken = default);
    Task<ResponseMessage<IEnumerable<TicketOrderListDto>>> GetPagedListAsync(TicketOrderGetPagedListRequest request, CancellationToken cancellationToken = default);
}

