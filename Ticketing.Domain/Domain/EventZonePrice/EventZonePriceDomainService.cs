using Ticketing.Application.Model.DTOs;
using Ticketing.Domain.Domain.EventZonePrice.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.EventZonePrice.Request;
using Ticketing.Infrastructure.DTOs.EventZonePrice.Response;
using Ticketing.Infrastructure.Entities;
using Ticketing.Infrastructure.Entities.EventZonePrice.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Repositories.EventZonePrice;

namespace Ticketing.Domain.Domain.EventZonePrice;

public class EventZonePriceDomainService(IEventZonePriceUnitOfWork unitOfWork)
    : BaseService<IEventZonePriceRepository, EventZonePriceEntity>(unitOfWork.EventZonePrice, TicketingTypeEnum.EventZonePrice),
        IEventZonePriceDomainService
{
    public async Task<ResponseMessage<int>> InsertAsync(EventZonePriceEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.InsertAsync(new
            {
                event_zone_id = entity.event_zone_id,
                price = entity.price,
                currency = entity.currency,
                start_time = entity.start_time,
                end_time = entity.end_time,
                is_active = entity.is_active,
                created_by = entity.created_by
            }, cancellationToken)!.ToIntAsync();

            if (result is not > 0)
                throw new Exception("Thêm giá vé vùng sự kiện thất bại");

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return new ResponseMessage<int>().MessageSuccess(result ?? 0, "Thêm giá vé vùng sự kiện thành công");
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<int>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> UpdateAsync(EventZonePriceEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.UpdateAsync(new
            {
                event_zone_price_id = entity.event_zone_price_id,
                event_zone_id = entity.event_zone_id,
                price = entity.price,
                currency = entity.currency,
                start_time = entity.start_time,
                end_time = entity.end_time,
                is_active = entity.is_active,
                updated_by = entity.updated_by
            }, cancellationToken)!.ToBoolAsync();

            if (!result)
                throw new Exception("Cập nhật giá vé vùng sự kiện thất bại");

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return new ResponseMessage<bool>().MessageSuccess(true, "Cập nhật giá vé vùng sự kiện thành công");
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> DeleteAsync(EventZonePriceEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.DeleteAsync(new
            {
                event_zone_price_id = entity.event_zone_price_id,
                updated_by = entity.updated_by
            }, cancellationToken)!.ToBoolAsync();

            if (!result)
                throw new Exception("Xóa giá vé vùng sự kiện thất bại");

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return new ResponseMessage<bool>().MessageSuccess(true, "Xóa giá vé vùng sự kiện thành công");
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<EventZonePriceDetailDto?>> GetByIdAsync(
        EventZonePriceGetByIdRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetAsync<EventZonePriceDetailDto>(new
            {
                event_zone_price_id = request.event_zone_price_id
            }, cancellationToken);

            if (result is null)
                return new ResponseMessage<EventZonePriceDetailDto?>().MessageWarning("Không tìm thấy thông tin giá vé vùng sự kiện");

            return new ResponseMessage<EventZonePriceDetailDto?>().MessageSuccess(result, "Lấy chi tiết giá vé vùng sự kiện thành công");
        }
        catch (Exception e)
        {
            return new ResponseMessage<EventZonePriceDetailDto?>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<EventZonePriceListDto>>> GetPagedListAsync(
        EventZonePriceGetPagedListRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetPagedAsync<EventZonePriceListDto>(new
            {
                pagesize = request.pagesize,
                offset = request.offset,
                keysearch = request.keysearch,
                event_zone_id = request.event_zone_id,
                is_active = request.is_active
            }, cancellationToken);

            return new ResponseMessage<IEnumerable<EventZonePriceListDto>>().MessageSuccess(result ?? [], "Lấy danh sách giá vé vùng sự kiện thành công");
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<EventZonePriceListDto>>().MessageError(e.Message);
        }
    }
}

