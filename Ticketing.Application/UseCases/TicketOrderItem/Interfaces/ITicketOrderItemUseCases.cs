using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.TicketOrderItem.Request;
using Ticketing.Infrastructure.DTOs.TicketOrderItem.Response;

namespace Ticketing.Application.UseCases.TicketOrderItem.Interfaces;

public interface ITicketOrderItemUseCases
{
    Task<ResponseMessage<int>?> InsertAsync(TicketOrderItemCreateRequest request, long? userLogin, CancellationToken cancellationToken = default);
    Task<ResponseMessage<bool>> UpdateAsync(TicketOrderItemUpdateRequest request, long? userLogin, CancellationToken cancellationToken = default);
    Task<ResponseMessage<bool>> DeleteAsync(TicketOrderItemDeleteRequest request, long? userLogin, CancellationToken cancellationToken = default);
    Task<ResponseMessage<TicketOrderItemDetailDto?>> GetByIdAsync(TicketOrderItemGetByIdRequest request, long? userLogin, CancellationToken cancellationToken = default);
    Task<ResponseMessage<IEnumerable<TicketOrderItemListDto>>> GetPagedListAsync(TicketOrderItemGetPagedListRequest request, long? userLogin, CancellationToken cancellationToken = default);
}

