using Ticketing.Application.Model.DTOs;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.Ticket.Request;
using Ticketing.Infrastructure.DTOs.Ticket.Response;
using Ticketing.Infrastructure.Entities.Ticket.Response;

namespace Ticketing.Domain.Domain.Ticket.Interfaces;

public interface ITicketDomainService
{
    Task<ResponseMessage<int>> InsertAsync(TicketEntity entity, CancellationToken cancellationToken = default);
    Task<ResponseMessage<bool>> UpdateAsync(TicketEntity entity, CancellationToken cancellationToken = default);
    Task<ResponseMessage<bool>> DeleteAsync(TicketEntity entity, CancellationToken cancellationToken = default);
    Task<ResponseMessage<TicketDetailDto?>> GetByIdAsync(TicketGetByIdRequest request, CancellationToken cancellationToken = default);
    Task<ResponseMessage<IEnumerable<TicketListDto>>> GetPagedListAsync(TicketGetPagedListRequest request, CancellationToken cancellationToken = default);
}

