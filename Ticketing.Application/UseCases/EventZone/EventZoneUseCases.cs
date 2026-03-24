using Ticketing.Application.UseCases.EventZone.Interfaces;
using Ticketing.Domain.Domain.EventZone.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.EventZone.Request;
using Ticketing.Infrastructure.DTOs.EventZone.Response;
using Ticketing.Infrastructure.Entities.EventZone.Response;

namespace Ticketing.Application.UseCases.EventZone;

public class EventZoneUseCases(IEventZoneDomainService eventZoneDomain) : IEventZoneUseCases
{
    public async Task<ResponseMessage<int>?> InsertAsync(EventZoneCreateRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<int>();

        try
        {
            var insert = await eventZoneDomain.InsertAsync(new EventZoneEntity
            {
                event_id = request.event_id,
                zone_code = request.zone_code,
                zone_name = request.zone_name,
                color_hex = request.color_hex,
                description = request.description,
                display_order = request.display_order,
                status = request.status,
                created_by = userLogin
            }, cancellationToken);

            if (!insert.issuccess)
                return new ResponseMessage<int>().MessageError(insert.message ?? "Thêm mới vùng sự kiện thất bại");

            return new ResponseMessage<int>().MessageSuccess(insert.data, "Thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> UpdateAsync(EventZoneUpdateRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<bool>();

        try
        {
            var update = await eventZoneDomain.UpdateAsync(new EventZoneEntity
            {
                event_zone_id = request.event_zone_id,
                event_id = request.event_id,
                zone_code = request.zone_code,
                zone_name = request.zone_name,
                color_hex = request.color_hex,
                description = request.description,
                display_order = request.display_order,
                status = request.status,
                updated_by = userLogin
            }, cancellationToken);

            if (!update.issuccess)
                return new ResponseMessage<bool>().MessageError(update.message ?? "Cập nhật vùng sự kiện thất bại");

            return new ResponseMessage<bool>().MessageSuccess(true, "Thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> DeleteAsync(EventZoneDeleteRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<bool>();

        try
        {
            var delete = await eventZoneDomain.DeleteAsync(new EventZoneEntity
            {
                event_zone_id = request.event_zone_id,
                updated_by = userLogin
            }, cancellationToken);

            if (!delete.issuccess)
                return new ResponseMessage<bool>().MessageError(delete.message ?? "Xóa vùng sự kiện thất bại");

            return new ResponseMessage<bool>().MessageSuccess(true, "Thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<EventZoneDetailDto?>> GetByIdAsync(EventZoneGetByIdRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        try
        {
            return await eventZoneDomain.GetByIdAsync(new EventZoneGetByIdRequest
            {
                event_zone_id = request.event_zone_id
            }, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<EventZoneDetailDto?>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<EventZoneListDto>>> GetPagedListAsync(EventZoneGetPagedListRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        try
        {
            return await eventZoneDomain.GetPagedListAsync(new EventZoneGetPagedListRequest
            {
                pagesize = request.pagesize,
                offset = request.offset,
                keysearch = request.keysearch,
                event_id = request.event_id,
                status = request.status
            }, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<EventZoneListDto>>().MessageError(e.Message);
        }
    }
}

