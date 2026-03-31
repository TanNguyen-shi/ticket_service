using Ticketing.Application.Model.DTOs;
using Ticketing.Domain.Constants;
using Ticketing.Domain.Domain.TicketOrderItem.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.TicketOrderItem.Request;
using Ticketing.Infrastructure.DTOs.TicketOrderItem.Response;
using Ticketing.Infrastructure.Entities;
using Ticketing.Infrastructure.Entities.TicketOrderItem.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Repositories.TicketOrderItem;
using Ticketing.Infrastructure.Repositories.Ticketing;

namespace Ticketing.Domain.Domain.TicketOrderItem;

public class TicketOrderItemDomainService(ITicketingUnitOfWork unitOfWork)
    : BaseService<ITicketOrderItemRepository, TicketOrderItemEntity>(unitOfWork.TicketOrderItem, TicketingTypeEnum.TicketOrderItem), ITicketOrderItemDomainService
{
    public async Task<ResponseMessage<int>> InsertAsync(TicketOrderItemEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);
        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.InsertAsync(new
            {
                order_id = entity.order_id,
                event_seat_inventory_id = entity.event_seat_inventory_id,
                seat_id = entity.seat_id,
                zone_id = entity.zone_id,
                unit_price = entity.unit_price,
                seat_label_snapshot = entity.seat_label_snapshot,
                zone_name_snapshot = entity.zone_name_snapshot,
                item_status = entity.item_status
            }, cancellationToken)!.ToIntAsync();

            if (result is not > 0)
                throw new Exception(DomainMessageConstants.TicketOrderItem.InsertError);

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<int>().MessageSuccess(result ?? 0, DomainMessageConstants.TicketOrderItem.InsertSuccess);
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<int>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> UpdateAsync(TicketOrderItemEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);
        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.UpdateAsync(new
            {
                order_item_id = entity.order_item_id,
                order_id = entity.order_id,
                event_seat_inventory_id = entity.event_seat_inventory_id,
                seat_id = entity.seat_id,
                zone_id = entity.zone_id,
                unit_price = entity.unit_price,
                seat_label_snapshot = entity.seat_label_snapshot,
                zone_name_snapshot = entity.zone_name_snapshot,
                item_status = entity.item_status
            }, cancellationToken)!.ToBoolAsync();

            if (!result)
                throw new Exception(DomainMessageConstants.TicketOrderItem.UpdateError);

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageSuccess(true, DomainMessageConstants.TicketOrderItem.UpdateSuccess);
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> DeleteAsync(TicketOrderItemEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);
        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);
            var result = await _repository.DeleteAsync(new { order_item_id = entity.order_item_id }, cancellationToken)!.ToBoolAsync();

            if (!result)
                throw new Exception(DomainMessageConstants.TicketOrderItem.DeleteError);

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageSuccess(true, DomainMessageConstants.TicketOrderItem.DeleteSuccess);
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<TicketOrderItemDetailDto?>> GetByIdAsync(TicketOrderItemGetByIdRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetAsync<TicketOrderItemDetailDto>(new { order_item_id = request.order_item_id }, cancellationToken);
            if (result is null)
                return new ResponseMessage<TicketOrderItemDetailDto?>().MessageWarning(DomainMessageConstants.TicketOrderItem.NotFound);
            return new ResponseMessage<TicketOrderItemDetailDto?>().MessageSuccess(result, DomainMessageConstants.TicketOrderItem.GetDetailSuccess);
        }
        catch (Exception e)
        {
            return new ResponseMessage<TicketOrderItemDetailDto?>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<TicketOrderItemListDto>>> GetPagedListAsync(TicketOrderItemGetPagedListRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetPagedAsync<TicketOrderItemListDto>(new
            {
                pagesize = request.pagesize,
                offset = request.offset,
                keysearch = request.keysearch,
                order_id = request.order_id,
                zone_id = request.zone_id,
                item_status = request.item_status
            }, cancellationToken);

            return new ResponseMessage<IEnumerable<TicketOrderItemListDto>>().MessageSuccess(result ?? [], DomainMessageConstants.TicketOrderItem.GetListSuccess);
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<TicketOrderItemListDto>>().MessageError(e.Message);
        }
    }
}
