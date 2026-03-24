using Ticketing.Application.UseCases.Ticket.Interfaces;
using Ticketing.Domain.Domain.Ticket.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.Ticket.Request;
using Ticketing.Infrastructure.DTOs.Ticket.Response;
using Ticketing.Infrastructure.Entities.Ticket.Response;

namespace Ticketing.Application.UseCases.Ticket;

public class TicketUseCases(ITicketDomainService ticketDomain) : ITicketUseCases
{
    public async Task<ResponseMessage<int>?> InsertAsync(TicketCreateRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<int>();
        try
        {
            var insert = await ticketDomain.InsertAsync(new TicketEntity
            {
                ticket_code = request.ticket_code,
                order_item_id = request.order_item_id,
                event_id = request.event_id,
                user_id = request.user_id,
                seat_id = request.seat_id,
                seat_label_snapshot = request.seat_label_snapshot,
                zone_name_snapshot = request.zone_name_snapshot,
                event_name_snapshot = request.event_name_snapshot,
                ticket_status = request.ticket_status,
                issued_at = request.issued_at,
                checked_in_at = request.checked_in_at
            }, cancellationToken);

            if (!insert.issuccess)
                return new ResponseMessage<int>().MessageError(insert.message ?? "Thêm mới vé thất bại");

            return new ResponseMessage<int>().MessageSuccess(insert.data, "Thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> UpdateAsync(TicketUpdateRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<bool>();
        try
        {
            var update = await ticketDomain.UpdateAsync(new TicketEntity
            {
                ticket_id = request.ticket_id,
                ticket_code = request.ticket_code,
                order_item_id = request.order_item_id,
                event_id = request.event_id,
                user_id = request.user_id,
                seat_id = request.seat_id,
                seat_label_snapshot = request.seat_label_snapshot,
                zone_name_snapshot = request.zone_name_snapshot,
                event_name_snapshot = request.event_name_snapshot,
                ticket_status = request.ticket_status,
                issued_at = request.issued_at,
                checked_in_at = request.checked_in_at
            }, cancellationToken);

            if (!update.issuccess)
                return new ResponseMessage<bool>().MessageError(update.message ?? "Cập nhật vé thất bại");

            return new ResponseMessage<bool>().MessageSuccess(true, "Thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> DeleteAsync(TicketDeleteRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<bool>();
        try
        {
            var delete = await ticketDomain.DeleteAsync(new TicketEntity
            {
                ticket_id = request.ticket_id
            }, cancellationToken);

            if (!delete.issuccess)
                return new ResponseMessage<bool>().MessageError(delete.message ?? "Xóa vé thất bại");

            return new ResponseMessage<bool>().MessageSuccess(true, "Thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<TicketDetailDto?>> GetByIdAsync(TicketGetByIdRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        try
        {
            return await ticketDomain.GetByIdAsync(new TicketGetByIdRequest { ticket_id = request.ticket_id }, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<TicketDetailDto?>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<TicketListDto>>> GetPagedListAsync(TicketGetPagedListRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        try
        {
            return await ticketDomain.GetPagedListAsync(new TicketGetPagedListRequest
            {
                pagesize = request.pagesize,
                offset = request.offset,
                keysearch = request.keysearch,
                event_id = request.event_id,
                user_id = request.user_id,
                ticket_status = request.ticket_status
            }, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<TicketListDto>>().MessageError(e.Message);
        }
    }
}

