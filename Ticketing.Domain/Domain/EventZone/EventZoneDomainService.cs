using Ticketing.Application.Model.DTOs;
using Ticketing.Domain.Domain.EventZone.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.EventZone.Request;
using Ticketing.Infrastructure.DTOs.EventZone.Response;
using Ticketing.Infrastructure.Entities;
using Ticketing.Infrastructure.Entities.EventZone.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Repositories.EventZone;

namespace Ticketing.Domain.Domain.EventZone;

public class EventZoneDomainService(IEventZoneUnitOfWork unitOfWork)
    : BaseService<IEventZoneRepository, EventZoneEntity>(unitOfWork.EventZone, TicketingTypeEnum.EventZone), IEventZoneDomainService
{
    public async Task<ResponseMessage<int>> InsertAsync(EventZoneEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.InsertAsync(new
            {
                event_id = entity.event_id,
                zone_code = entity.zone_code,
                zone_name = entity.zone_name,
                color_hex = entity.color_hex,
                description = entity.description,
                display_order = entity.display_order,
                status = entity.status,
                created_by = entity.created_by
            }, cancellationToken)!.ToIntAsync();

            if (result is not > 0)
                throw new Exception("Thêm mới vùng sự kiện thất bại");

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return new ResponseMessage<int>().MessageSuccess(result ?? 0, "Thêm vùng sự kiện thành công");
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<int>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> UpdateAsync(EventZoneEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.UpdateAsync(new
            {
                event_zone_id = entity.event_zone_id,
                event_id = entity.event_id,
                zone_code = entity.zone_code,
                zone_name = entity.zone_name,
                color_hex = entity.color_hex,
                description = entity.description,
                display_order = entity.display_order,
                status = entity.status,
                updated_by = entity.updated_by
            }, cancellationToken)!.ToBoolAsync();

            if (!result)
                throw new Exception("Cập nhật vùng sự kiện thất bại");

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return new ResponseMessage<bool>().MessageSuccess(true, "Cập nhật vùng sự kiện thành công");
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> DeleteAsync(EventZoneEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.DeleteAsync(new
            {
                event_zone_id = entity.event_zone_id,
                updated_by = entity.updated_by
            }, cancellationToken)!.ToBoolAsync();

            if (!result)
                throw new Exception("Xóa vùng sự kiện thất bại");

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return new ResponseMessage<bool>().MessageSuccess(true, "Xóa vùng sự kiện thành công");
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<EventZoneDetailDto?>> GetByIdAsync(EventZoneGetByIdRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetAsync<EventZoneDetailDto>(new
            {
                event_zone_id = request.event_zone_id
            }, cancellationToken);

            if (result is null)
                return new ResponseMessage<EventZoneDetailDto?>().MessageWarning("Không tìm thấy thông tin vùng sự kiện");

            return new ResponseMessage<EventZoneDetailDto?>().MessageSuccess(result, "Lấy chi tiết vùng sự kiện thành công");
        }
        catch (Exception e)
        {
            return new ResponseMessage<EventZoneDetailDto?>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<EventZoneListDto>>> GetPagedListAsync(EventZoneGetPagedListRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetPagedAsync<EventZoneListDto>(new
            {
                pagesize = request.pagesize,
                offset = request.offset,
                keysearch = request.keysearch,
                event_id = request.event_id,
                status = request.status
            }, cancellationToken);

            return new ResponseMessage<IEnumerable<EventZoneListDto>>().MessageSuccess(result ?? [], "Lấy danh sách vùng sự kiện thành công");
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<EventZoneListDto>>().MessageError(e.Message);
        }
    }
}

