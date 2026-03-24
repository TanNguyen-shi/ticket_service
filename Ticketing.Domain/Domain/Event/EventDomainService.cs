using Ticketing.Application.Model.DTOs;
using Ticketing.Domain.Domain.Event.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.Event.Request;
using Ticketing.Infrastructure.DTOs.Event.Response;
using Ticketing.Infrastructure.Entities;
using Ticketing.Infrastructure.Entities.Event.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Repositories.Event;

namespace Ticketing.Domain.Domain.Event;

public class EventDomainService(IEventUnitOfWork unitOfWork)
    : BaseService<IEventRepository, EventEntity>(unitOfWork.Event, TicketingTypeEnum.Event), IEventDomainService
{
    public async Task<ResponseMessage<int>> InsertAsync(EventEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.InsertAsync(new
            {
                event_code = entity.event_code,
                event_name = entity.event_name,
                description = entity.description,
                venue_id = entity.venue_id,
                banner_url = entity.banner_url,
                start_time = entity.start_time,
                end_time = entity.end_time,
                sale_start_time = entity.sale_start_time,
                sale_end_time = entity.sale_end_time,
                status = entity.status,
                published_at = entity.published_at,
                on_sale_at = entity.on_sale_at,
                created_by = entity.created_by
            }, cancellationToken)!.ToIntAsync();

            if (result is not > 0)
                throw new Exception("Thêm mới sự kiện thất bại");

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return new ResponseMessage<int>().MessageSuccess(result ?? 0, "Thành công");
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<int>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> UpdateAsync(EventEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.UpdateAsync(new
            {
                event_id = entity.event_id,
                event_code = entity.event_code,
                event_name = entity.event_name,
                description = entity.description,
                venue_id = entity.venue_id,
                banner_url = entity.banner_url,
                start_time = entity.start_time,
                end_time = entity.end_time,
                sale_start_time = entity.sale_start_time,
                sale_end_time = entity.sale_end_time,
                status = entity.status,
                published_at = entity.published_at,
                on_sale_at = entity.on_sale_at,
                updated_by = entity.updated_by
            }, cancellationToken)!.ToBoolAsync();

            if (!result)
                throw new Exception("Cập nhật sự kiện thất bại");

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return new ResponseMessage<bool>().MessageSuccess(true, "Thành công");
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> DeleteAsync(EventEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.DeleteAsync(new
            {
                event_id = entity.event_id,
                updated_by = entity.updated_by
            }, cancellationToken)!.ToBoolAsync();

            if (!result)
                throw new Exception("Xóa dữ liệu sự kiện thất bại");

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return new ResponseMessage<bool>().MessageSuccess(true, "Thành công");
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<EventDetailDto?>> GetByIdAsync(EventGetByIdRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetAsync<EventDetailDto>(new
            {
                event_id = request.event_id
            }, cancellationToken);

            if (result is null)
                return new ResponseMessage<EventDetailDto?>().MessageWarning("Không tìm thấy dữ liệu");

            return new ResponseMessage<EventDetailDto?>().MessageSuccess(result, "Thành công");
        }
        catch (Exception e)
        {
            return new ResponseMessage<EventDetailDto?>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<EventListDto>>> GetPagedListAsync(EventGetPagedListRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetPagedAsync<EventListDto>(new
            {
                pagesize = request.pagesize,
                offset = request.offset,
                keysearch = request.keysearch,
                status = request.status,
                venue_id = request.venue_id
            }, cancellationToken);

            return new ResponseMessage<IEnumerable<EventListDto>>().MessageSuccess(result ?? [], "Thành công");
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<EventListDto>>().MessageError(e.Message);
        }
    }
}

