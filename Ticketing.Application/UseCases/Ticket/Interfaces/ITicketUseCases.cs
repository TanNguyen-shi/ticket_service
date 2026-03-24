using Ticketing.Application.Model.DTOs;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.Ticket.Request;
using Ticketing.Infrastructure.DTOs.Ticket.Response;

namespace Ticketing.Application.UseCases.Ticket.Interfaces;

public interface ITicketUseCases
{
    Task<ResponseMessage<int>?> InsertAsync(TicketCreateRequest request, long? userLogin, CancellationToken cancellationToken = default);
    Task<ResponseMessage<bool>> UpdateAsync(TicketUpdateRequest request, long? userLogin, CancellationToken cancellationToken = default);
    Task<ResponseMessage<bool>> DeleteAsync(TicketDeleteRequest request, long? userLogin, CancellationToken cancellationToken = default);
    Task<ResponseMessage<TicketDetailDto?>> GetByIdAsync(TicketGetByIdRequest request, long? userLogin, CancellationToken cancellationToken = default);
    Task<ResponseMessage<IEnumerable<TicketListDto>>> GetPagedListAsync(TicketGetPagedListRequest request, long? userLogin, CancellationToken cancellationToken = default);
}

