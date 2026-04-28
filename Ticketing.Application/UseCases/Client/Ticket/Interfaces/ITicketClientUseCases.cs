using Ticketing.Application.Model.DTOs;
using Ticketing.Infrastructure.DTOs.Client.Ticket.Response;

namespace Ticketing.Application.UseCases.Client.Ticket.Interfaces;

public interface ITicketClientUseCases
{
    Task<ResponseMessage<IEnumerable<TicketListItemDto>>> GetMyTicketsAsync(long customerId, CancellationToken cancellationToken = default);
    Task<ResponseMessage<TicketDetailDto>> GetTicketDetailAsync(long ticketId, long customerId, CancellationToken cancellationToken = default);
}
