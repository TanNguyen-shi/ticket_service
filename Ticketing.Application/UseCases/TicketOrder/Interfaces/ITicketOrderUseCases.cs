using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.TicketOrder.Request;
using Ticketing.Infrastructure.DTOs.TicketOrder.Response;

namespace Ticketing.Application.UseCases.TicketOrder.Interfaces;

public interface ITicketOrderUseCases
{
    Task<ResponseMessage<int>?> InsertAsync(TicketOrderCreateRequest request, long? userLogin, CancellationToken cancellationToken = default);
    Task<ResponseMessage<bool>> UpdateAsync(TicketOrderUpdateRequest request, long? userLogin, CancellationToken cancellationToken = default);
    Task<ResponseMessage<bool>> DeleteAsync(TicketOrderDeleteRequest request, long? userLogin, CancellationToken cancellationToken = default);
    Task<ResponseMessage<TicketOrderDetailDto?>> GetByIdAsync(TicketOrderGetByIdRequest request, long? userLogin, CancellationToken cancellationToken = default);
    Task<ResponseMessage<IEnumerable<TicketOrderListDto>>> GetPagedListAsync(TicketOrderGetPagedListRequest request, long? userLogin, CancellationToken cancellationToken = default);
}

