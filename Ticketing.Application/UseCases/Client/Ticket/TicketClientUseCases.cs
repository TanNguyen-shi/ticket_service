using Ticketing.Application.Model.DTOs;
using Ticketing.Application.UseCases.Client.Ticket.Interfaces;
using Ticketing.Infrastructure.DTOs.Client.Ticket.Response;
using Ticketing.Infrastructure.Repositories.Ticket;

namespace Ticketing.Application.UseCases.Client.Ticket;

public class TicketClientUseCases(ITicketRepository ticketRepository) : ITicketClientUseCases
{
    public async Task<ResponseMessage<IEnumerable<TicketListItemDto>>> GetMyTicketsAsync(
        long customerId,
        CancellationToken cancellationToken = default)
    {
        if (customerId <= 0)
            return new ResponseMessage<IEnumerable<TicketListItemDto>>().MessageError("Vui lòng đăng nhập để tiếp tục");

        try
        {
            var tickets = await ticketRepository.GetByCustomerIdAsync<TicketListItemDto>(
                new { customer_id = customerId },
                cancellationToken);

            return new ResponseMessage<IEnumerable<TicketListItemDto>>().MessageSuccess(tickets, "Lấy danh sách vé thành công");
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<TicketListItemDto>>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<TicketDetailDto>> GetTicketDetailAsync(
        long ticketId,
        long customerId,
        CancellationToken cancellationToken = default)
    {
        if (ticketId <= 0)
            return new ResponseMessage<TicketDetailDto>().MessageError("Thông tin vé không hợp lệ");

        if (customerId <= 0)
            return new ResponseMessage<TicketDetailDto>().MessageError("Vui lòng đăng nhập để tiếp tục");

        try
        {
            // Stored proc ticket_getbyid lọc theo cả ticket_id và customer_id để đảm bảo ownership
            var ticket = await ticketRepository.GetAsync<TicketDetailDto>(
                new { ticket_id = ticketId, customer_id = customerId },
                cancellationToken);

            if (ticket is null)
                return new ResponseMessage<TicketDetailDto>().MessageError("Không tìm thấy vé hoặc bạn không có quyền xem vé này");

            return new ResponseMessage<TicketDetailDto>().MessageSuccess(ticket, "Lấy thông tin vé thành công");
        }
        catch (Exception e)
        {
            return new ResponseMessage<TicketDetailDto>().MessageError(e.Message);
        }
    }
}
