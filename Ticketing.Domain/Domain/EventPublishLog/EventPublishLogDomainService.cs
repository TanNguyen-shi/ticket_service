using Ticketing.Application.Model.DTOs;
using Ticketing.Domain.Domain.EventPublishLog.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.EventPublishLog.Request;
using Ticketing.Infrastructure.DTOs.EventPublishLog.Response;
using Ticketing.Infrastructure.Entities;
using Ticketing.Infrastructure.Entities.EventPublishLog.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Repositories.EventPublishLog;

namespace Ticketing.Domain.Domain.EventPublishLog;

public class EventPublishLogDomainService(IEventPublishLogUnitOfWork unitOfWork)
    : BaseService<IEventPublishLogRepository, EventPublishLogEntity>(
        unitOfWork.EventPublishLog,
        TicketingTypeEnum.EventPublishLog), IEventPublishLogDomainService
{
    public async Task<ResponseMessage<int>> InsertAsync(EventPublishLogEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.InsertAsync(new
            {
                event_id = entity.event_id,
                action = entity.action,
                old_status = entity.old_status,
                new_status = entity.new_status,
                changed_by = entity.changed_by,
                note = entity.note
            }, cancellationToken)!.ToIntAsync();

            if (result is not > 0)
                throw new Exception("Thêm mới lịch sử publish thất bại");

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return new ResponseMessage<int>().MessageSuccess(result ?? 0, "Thành công");
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<int>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> UpdateAsync(EventPublishLogEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.UpdateAsync(new
            {
                event_publish_log_id = entity.event_publish_log_id,
                event_id = entity.event_id,
                action = entity.action,
                old_status = entity.old_status,
                new_status = entity.new_status,
                changed_by = entity.changed_by,
                note = entity.note
            }, cancellationToken)!.ToBoolAsync();

            if (!result)
                throw new Exception("Cập nhật lịch sử publish thất bại");

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return new ResponseMessage<bool>().MessageSuccess(true, "Thành công");
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> DeleteAsync(EventPublishLogEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.DeleteAsync(new
            {
                event_publish_log_id = entity.event_publish_log_id
            }, cancellationToken)!.ToBoolAsync();

            if (!result)
                throw new Exception("Xóa lịch sử publish thất bại");

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return new ResponseMessage<bool>().MessageSuccess(true, "Thành công");
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<EventPublishLogDetailDto?>> GetByIdAsync(
        EventPublishLogGetByIdRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetAsync<EventPublishLogDetailDto>(new
            {
                event_publish_log_id = request.event_publish_log_id
            }, cancellationToken);

            if (result is null)
                return new ResponseMessage<EventPublishLogDetailDto?>().MessageWarning("Không tìm thấy dữ liệu");

            return new ResponseMessage<EventPublishLogDetailDto?>().MessageSuccess(result, "Thành công");
        }
        catch (Exception e)
        {
            return new ResponseMessage<EventPublishLogDetailDto?>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<EventPublishLogListDto>>> GetPagedListAsync(
        EventPublishLogGetPagedListRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetPagedAsync<EventPublishLogListDto>(new
            {
                pagesize = request.pagesize,
                offset = request.offset,
                keysearch = request.keysearch,
                event_id = request.event_id,
                action = request.action,
                new_status = request.new_status
            }, cancellationToken);

            return new ResponseMessage<IEnumerable<EventPublishLogListDto>>().MessageSuccess(result ?? [], "Thành công");
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<EventPublishLogListDto>>().MessageError(e.Message);
        }
    }
}

