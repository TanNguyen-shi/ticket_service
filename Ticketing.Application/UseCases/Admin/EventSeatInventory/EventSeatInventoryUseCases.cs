using Ticketing.Application.Model.DTOs;
using Ticketing.Application.UseCases.EventSeatInventory.Interfaces;
using Ticketing.Domain.Domain.EventSeatInventory.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.EventSeatInventory.Request;
using Ticketing.Infrastructure.DTOs.EventSeatInventory.Response;
using Ticketing.Infrastructure.Entities.EventSeatInventory.Response;

namespace Ticketing.Application.UseCases.EventSeatInventory;

public class EventSeatInventoryUseCases(IEventSeatInventoryDomainService eventSeatInventoryDomain) : IEventSeatInventoryUseCases
{
    public async Task<ResponseMessage<int>?> InsertAsync(
        EventSeatInventoryCreateRequest request,
        long? userLogin,
        CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<int>();

        try
        {
            var insert = await eventSeatInventoryDomain.InsertAsync(new EventSeatInventoryEntity
            {
                event_id = request.event_id,
                seat_id = request.seat_id,
                event_zone_id = request.event_zone_id,
                seat_status = request.seat_status,
                current_hold_id = request.current_hold_id,
                current_order_item_id = request.current_order_item_id,
                base_price = request.base_price,
                version = request.version
            }, cancellationToken);

            if (!insert.issuccess)
                return new ResponseMessage<int>().MessageError(insert.message ?? "Thêm mới tồn kho ghế sự kiện thất bại");

            return new ResponseMessage<int>().MessageSuccess(insert.data, "Thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> UpdateAsync(
        EventSeatInventoryUpdateRequest request,
        long? userLogin,
        CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<bool>();

        try
        {
            var update = await eventSeatInventoryDomain.UpdateAsync(new EventSeatInventoryEntity
            {
                event_seat_inventory_id = request.event_seat_inventory_id,
                event_id = request.event_id,
                seat_id = request.seat_id,
                event_zone_id = request.event_zone_id,
                seat_status = request.seat_status,
                current_hold_id = request.current_hold_id,
                current_order_item_id = request.current_order_item_id,
                base_price = request.base_price,
                version = request.version
            }, cancellationToken);

            if (!update.issuccess)
                return new ResponseMessage<bool>().MessageError(update.message ?? "Cập nhật tồn kho ghế sự kiện thất bại");

            return new ResponseMessage<bool>().MessageSuccess(true, "Thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> DeleteAsync(
        EventSeatInventoryDeleteRequest request,
        long? userLogin,
        CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<bool>();

        try
        {
            var delete = await eventSeatInventoryDomain.DeleteAsync(new EventSeatInventoryEntity
            {
                event_seat_inventory_id = request.event_seat_inventory_id
            }, cancellationToken);

            if (!delete.issuccess)
                return new ResponseMessage<bool>().MessageError(delete.message ?? "Xóa tồn kho ghế sự kiện thất bại");

            return new ResponseMessage<bool>().MessageSuccess(true, "Thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<EventSeatInventoryDetailDto?>> GetByIdAsync(
        EventSeatInventoryGetByIdRequest request,
        long? userLogin,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await eventSeatInventoryDomain.GetByIdAsync(new EventSeatInventoryGetByIdRequest
            {
                event_seat_inventory_id = request.event_seat_inventory_id
            }, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<EventSeatInventoryDetailDto?>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<EventSeatInventoryListDto>>> GetPagedListAsync(
        EventSeatInventoryGetPagedListRequest request,
        long? userLogin,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await eventSeatInventoryDomain.GetPagedListAsync(new EventSeatInventoryGetPagedListRequest
            {
                pagesize = request.pagesize,
                offset = request.offset,
                keysearch = request.keysearch,
                event_id = request.event_id,
                event_zone_id = request.event_zone_id,
                seat_status = request.seat_status
            }, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<EventSeatInventoryListDto>>().MessageError(e.Message);
        }
    }
}

