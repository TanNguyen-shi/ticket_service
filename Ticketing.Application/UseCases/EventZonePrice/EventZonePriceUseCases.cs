using Ticketing.Application.Model.DTOs;
using Ticketing.Application.UseCases.EventZonePrice.Interfaces;
using Ticketing.Domain.Domain.EventZonePrice.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.EventZonePrice.Request;
using Ticketing.Infrastructure.DTOs.EventZonePrice.Response;
using Ticketing.Infrastructure.Entities.EventZonePrice.Response;

namespace Ticketing.Application.UseCases.EventZonePrice;

public class EventZonePriceUseCases(IEventZonePriceDomainService eventZonePriceDomain) : IEventZonePriceUseCases
{
    public async Task<ResponseMessage<int>?> InsertAsync(
        EventZonePriceCreateRequest request,
        long? userLogin,
        CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<int>();

        try
        {
            var insert = await eventZonePriceDomain.InsertAsync(new EventZonePriceEntity
            {
                event_zone_id = request.event_zone_id,
                price = request.price,
                currency = request.currency,
                start_time = request.start_time,
                end_time = request.end_time,
                is_active = request.is_active,
                created_by = userLogin
            }, cancellationToken);

            if (!insert.issuccess)
                return new ResponseMessage<int>().MessageError(insert.message ?? "Thêm giá vé vùng sự kiện thất bại");

            return new ResponseMessage<int>().MessageSuccess(insert.data, "Thêm giá vé vùng sự kiện thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> UpdateAsync(
        EventZonePriceUpdateRequest request,
        long? userLogin,
        CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<bool>();

        try
        {
            var update = await eventZonePriceDomain.UpdateAsync(new EventZonePriceEntity
            {
                event_zone_price_id = request.event_zone_price_id,
                event_zone_id = request.event_zone_id,
                price = request.price,
                currency = request.currency,
                start_time = request.start_time,
                end_time = request.end_time,
                is_active = request.is_active,
                updated_by = userLogin
            }, cancellationToken);

            if (!update.issuccess)
                return new ResponseMessage<bool>().MessageError(update.message ?? "Cập nhật giá vé vùng sự kiện thất bại");

            return new ResponseMessage<bool>().MessageSuccess(true, "Cập nhật giá vé vùng sự kiện thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> DeleteAsync(
        EventZonePriceDeleteRequest request,
        long? userLogin,
        CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<bool>();

        try
        {
            var delete = await eventZonePriceDomain.DeleteAsync(new EventZonePriceEntity
            {
                event_zone_price_id = request.event_zone_price_id,
                updated_by = userLogin
            }, cancellationToken);

            if (!delete.issuccess)
                return new ResponseMessage<bool>().MessageError(delete.message ?? "Xóa giá vé vùng sự kiện thất bại");

            return new ResponseMessage<bool>().MessageSuccess(true, "Xóa giá vé vùng sự kiện thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<EventZonePriceDetailDto?>> GetByIdAsync(
        EventZonePriceGetByIdRequest request,
        long? userLogin,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await eventZonePriceDomain.GetByIdAsync(new EventZonePriceGetByIdRequest
            {
                event_zone_price_id = request.event_zone_price_id
            }, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<EventZonePriceDetailDto?>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<EventZonePriceListDto>>> GetPagedListAsync(
        EventZonePriceGetPagedListRequest request,
        long? userLogin,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await eventZonePriceDomain.GetPagedListAsync(new EventZonePriceGetPagedListRequest
            {
                pagesize = request.pagesize,
                offset = request.offset,
                keysearch = request.keysearch,
                event_zone_id = request.event_zone_id,
                is_active = request.is_active
            }, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<EventZonePriceListDto>>().MessageError(e.Message);
        }
    }
}

