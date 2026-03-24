using Ticketing.Application.Model.DTOs;
using Ticketing.Application.UseCases.TicketOrder.Interfaces;
using Ticketing.Domain.Domain.TicketOrder.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.TicketOrder.Request;
using Ticketing.Infrastructure.DTOs.TicketOrder.Response;
using Ticketing.Infrastructure.Entities.TicketOrder.Response;

namespace Ticketing.Application.UseCases.TicketOrder;

public class TicketOrderUseCases(ITicketOrderDomainService ticketOrderDomain) : ITicketOrderUseCases
{
    public async Task<ResponseMessage<int>?> InsertAsync(TicketOrderCreateRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<int>();
        try
        {
            var insert = await ticketOrderDomain.InsertAsync(new TicketOrderEntity
            {
                order_code = request.order_code,
                event_id = request.event_id,
                user_id = request.user_id,
                hold_id = request.hold_id,
                total_amount = request.total_amount,
                discount_amount = request.discount_amount,
                final_amount = request.final_amount,
                order_status = request.order_status,
                expired_at = request.expired_at,
                paid_at = request.paid_at
            }, cancellationToken);

            if (!insert.issuccess)
                return new ResponseMessage<int>().MessageError(insert.message ?? "Thêm mới đơn hàng thất bại");

            return new ResponseMessage<int>().MessageSuccess(insert.data, "Thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> UpdateAsync(TicketOrderUpdateRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<bool>();
        try
        {
            var update = await ticketOrderDomain.UpdateAsync(new TicketOrderEntity
            {
                order_id = request.order_id,
                order_code = request.order_code,
                event_id = request.event_id,
                user_id = request.user_id,
                hold_id = request.hold_id,
                total_amount = request.total_amount,
                discount_amount = request.discount_amount,
                final_amount = request.final_amount,
                order_status = request.order_status,
                expired_at = request.expired_at,
                paid_at = request.paid_at
            }, cancellationToken);

            if (!update.issuccess)
                return new ResponseMessage<bool>().MessageError(update.message ?? "Cập nhật đơn hàng thất bại");

            return new ResponseMessage<bool>().MessageSuccess(true, "Thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> DeleteAsync(TicketOrderDeleteRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<bool>();
        try
        {
            var delete = await ticketOrderDomain.DeleteAsync(new TicketOrderEntity
            {
                order_id = request.order_id
            }, cancellationToken);

            if (!delete.issuccess)
                return new ResponseMessage<bool>().MessageError(delete.message ?? "Xóa đơn hàng thất bại");

            return new ResponseMessage<bool>().MessageSuccess(true, "Thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<TicketOrderDetailDto?>> GetByIdAsync(TicketOrderGetByIdRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        try
        {
            return await ticketOrderDomain.GetByIdAsync(new TicketOrderGetByIdRequest { order_id = request.order_id }, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<TicketOrderDetailDto?>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<TicketOrderListDto>>> GetPagedListAsync(TicketOrderGetPagedListRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        try
        {
            return await ticketOrderDomain.GetPagedListAsync(new TicketOrderGetPagedListRequest
            {
                pagesize = request.pagesize,
                offset = request.offset,
                keysearch = request.keysearch,
                event_id = request.event_id,
                user_id = request.user_id,
                order_status = request.order_status
            }, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<TicketOrderListDto>>().MessageError(e.Message);
        }
    }
}

