using Ticketing.Application.UseCases.TicketOrderItem.Interfaces;
using Ticketing.Domain.Domain.TicketOrderItem.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.TicketOrderItem.Request;
using Ticketing.Infrastructure.DTOs.TicketOrderItem.Response;
using Ticketing.Infrastructure.Entities.TicketOrderItem.Response;

namespace Ticketing.Application.UseCases.TicketOrderItem;

public class TicketOrderItemUseCases(ITicketOrderItemDomainService ticketOrderItemDomain) : ITicketOrderItemUseCases
{
    public async Task<ResponseMessage<int>?> InsertAsync(TicketOrderItemCreateRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<int>();
        try
        {
            var insert = await ticketOrderItemDomain.InsertAsync(new TicketOrderItemEntity
            {
                order_id = request.order_id,
                event_seat_inventory_id = request.event_seat_inventory_id,
                seat_id = request.seat_id,
                zone_id = request.zone_id,
                unit_price = request.unit_price,
                seat_label_snapshot = request.seat_label_snapshot,
                zone_name_snapshot = request.zone_name_snapshot,
                item_status = request.item_status
            }, cancellationToken);

            if (!insert.issuccess)
                return new ResponseMessage<int>().MessageError(insert.message ?? "Thêm mới chi tiết đơn hàng thất bại");

            return new ResponseMessage<int>().MessageSuccess(insert.data, "Thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> UpdateAsync(TicketOrderItemUpdateRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<bool>();
        try
        {
            var update = await ticketOrderItemDomain.UpdateAsync(new TicketOrderItemEntity
            {
                order_item_id = request.order_item_id,
                order_id = request.order_id,
                event_seat_inventory_id = request.event_seat_inventory_id,
                seat_id = request.seat_id,
                zone_id = request.zone_id,
                unit_price = request.unit_price,
                seat_label_snapshot = request.seat_label_snapshot,
                zone_name_snapshot = request.zone_name_snapshot,
                item_status = request.item_status
            }, cancellationToken);

            if (!update.issuccess)
                return new ResponseMessage<bool>().MessageError(update.message ?? "Cập nhật chi tiết đơn hàng thất bại");

            return new ResponseMessage<bool>().MessageSuccess(true, "Thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> DeleteAsync(TicketOrderItemDeleteRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<bool>();
        try
        {
            var delete = await ticketOrderItemDomain.DeleteAsync(new TicketOrderItemEntity
            {
                order_item_id = request.order_item_id
            }, cancellationToken);

            if (!delete.issuccess)
                return new ResponseMessage<bool>().MessageError(delete.message ?? "Xóa chi tiết đơn hàng thất bại");

            return new ResponseMessage<bool>().MessageSuccess(true, "Thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<TicketOrderItemDetailDto?>> GetByIdAsync(TicketOrderItemGetByIdRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        try
        {
            return await ticketOrderItemDomain.GetByIdAsync(new TicketOrderItemGetByIdRequest { order_item_id = request.order_item_id }, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<TicketOrderItemDetailDto?>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<TicketOrderItemListDto>>> GetPagedListAsync(TicketOrderItemGetPagedListRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        try
        {
            return await ticketOrderItemDomain.GetPagedListAsync(new TicketOrderItemGetPagedListRequest
            {
                pagesize = request.pagesize,
                offset = request.offset,
                keysearch = request.keysearch,
                order_id = request.order_id,
                zone_id = request.zone_id,
                item_status = request.item_status
            }, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<TicketOrderItemListDto>>().MessageError(e.Message);
        }
    }
}

