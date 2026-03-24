using Ticketing.Application.UseCases.EventPublishLog.Interfaces;
using Ticketing.Domain.Domain.EventPublishLog.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.EventPublishLog.Request;
using Ticketing.Infrastructure.DTOs.EventPublishLog.Response;
using Ticketing.Infrastructure.Entities.EventPublishLog.Response;

namespace Ticketing.Application.UseCases.EventPublishLog;

public class EventPublishLogUseCases(IEventPublishLogDomainService eventPublishLogDomain) : IEventPublishLogUseCases
{
    public async Task<ResponseMessage<int>?> InsertAsync(EventPublishLogCreateRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<int>();

        try
        {
            var insert = await eventPublishLogDomain.InsertAsync(new EventPublishLogEntity
            {
                event_id = request.event_id,
                action = request.action,
                old_status = request.old_status,
                new_status = request.new_status,
                changed_by = userLogin,
                note = request.note
            }, cancellationToken);

            if (!insert.issuccess)
                return new ResponseMessage<int>().MessageError(insert.message ?? "Thêm mới lịch sử publish thất bại");

            return new ResponseMessage<int>().MessageSuccess(insert.data, "Thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> UpdateAsync(EventPublishLogUpdateRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<bool>();

        try
        {
            var update = await eventPublishLogDomain.UpdateAsync(new EventPublishLogEntity
            {
                event_publish_log_id = request.event_publish_log_id,
                event_id = request.event_id,
                action = request.action,
                old_status = request.old_status,
                new_status = request.new_status,
                changed_by = userLogin,
                note = request.note
            }, cancellationToken);

            if (!update.issuccess)
                return new ResponseMessage<bool>().MessageError(update.message ?? "Cập nhật lịch sử publish thất bại");

            return new ResponseMessage<bool>().MessageSuccess(true, "Thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> DeleteAsync(EventPublishLogDeleteRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<bool>();

        try
        {
            var delete = await eventPublishLogDomain.DeleteAsync(new EventPublishLogEntity
            {
                event_publish_log_id = request.event_publish_log_id
            }, cancellationToken);

            if (!delete.issuccess)
                return new ResponseMessage<bool>().MessageError(delete.message ?? "Xóa lịch sử publish thất bại");

            return new ResponseMessage<bool>().MessageSuccess(true, "Thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<EventPublishLogDetailDto?>> GetByIdAsync(EventPublishLogGetByIdRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        try
        {
            return await eventPublishLogDomain.GetByIdAsync(new EventPublishLogGetByIdRequest
            {
                event_publish_log_id = request.event_publish_log_id
            }, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<EventPublishLogDetailDto?>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<EventPublishLogListDto>>> GetPagedListAsync(EventPublishLogGetPagedListRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        try
        {
            return await eventPublishLogDomain.GetPagedListAsync(new EventPublishLogGetPagedListRequest
            {
                pagesize = request.pagesize,
                offset = request.offset,
                keysearch = request.keysearch,
                event_id = request.event_id,
                action = request.action,
                new_status = request.new_status
            }, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<EventPublishLogListDto>>().MessageError(e.Message);
        }
    }
}

