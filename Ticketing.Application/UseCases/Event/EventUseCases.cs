using Ticketing.Application.Model.DTOs;
using Ticketing.Application.UseCases.Event.Interfaces;
using Ticketing.Domain.Domain.Event.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.Event.Request;
using Ticketing.Infrastructure.DTOs.Event.Response;
using Ticketing.Infrastructure.Entities.Event.Response;

namespace Ticketing.Application.UseCases.Event;

public class EventUseCases(IEventDomainService eventDomain) : IEventUseCases
{
    public async Task<ResponseMessage<int>?> InsertAsync(EventCreateRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<int>();

        try
        {
            var insert = await eventDomain.InsertAsync(new EventEntity
            {
                event_code = request.event_code,
                event_name = request.event_name,
                description = request.description,
                venue_id = request.venue_id,
                banner_url = request.banner_url,
                start_time = request.start_time,
                end_time = request.end_time,
                sale_start_time = request.sale_start_time,
                sale_end_time = request.sale_end_time,
                status = request.status,
                published_at = request.published_at,
                on_sale_at = request.on_sale_at,
                created_by = userLogin
            }, cancellationToken);

            if (!insert.issuccess)
                return new ResponseMessage<int>().MessageError(insert.message ?? "Thêm mới sự kiện thất bại");

            return new ResponseMessage<int>().MessageSuccess(insert.data, "Thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> UpdateAsync(EventUpdateRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<bool>();

        try
        {
            var update = await eventDomain.UpdateAsync(new EventEntity
            {
                event_id = request.event_id,
                event_code = request.event_code,
                event_name = request.event_name,
                description = request.description,
                venue_id = request.venue_id,
                banner_url = request.banner_url,
                start_time = request.start_time,
                end_time = request.end_time,
                sale_start_time = request.sale_start_time,
                sale_end_time = request.sale_end_time,
                status = request.status,
                published_at = request.published_at,
                on_sale_at = request.on_sale_at,
                updated_by = userLogin
            }, cancellationToken);

            if (!update.issuccess)
                return new ResponseMessage<bool>().MessageError(update.message ?? "Cập nhật sự kiện thất bại");

            return new ResponseMessage<bool>().MessageSuccess(true, "Thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> DeleteAsync(EventDeleteRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<bool>();

        try
        {
            var delete = await eventDomain.DeleteAsync(new EventEntity
            {
                event_id = request.event_id,
                updated_by = userLogin
            }, cancellationToken);

            if (!delete.issuccess)
                return new ResponseMessage<bool>().MessageError(delete.message ?? "Xóa sự kiện thất bại");

            return new ResponseMessage<bool>().MessageSuccess(true, "Thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<EventDetailDto?>> GetByIdAsync(EventGetByIdRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        try
        {
            return await eventDomain.GetByIdAsync(new EventGetByIdRequest
            {
                event_id = request.event_id
            }, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<EventDetailDto?>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<EventListDto>>> GetPagedListAsync(EventGetPagedListRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        try
        {
            return await eventDomain.GetPagedListAsync(new EventGetPagedListRequest
            {
                pagesize = request.pagesize,
                offset = request.offset,
                keysearch = request.keysearch,
                status = request.status,
                venue_id = request.venue_id
            }, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<EventListDto>>().MessageError(e.Message);
        }
    }
}

