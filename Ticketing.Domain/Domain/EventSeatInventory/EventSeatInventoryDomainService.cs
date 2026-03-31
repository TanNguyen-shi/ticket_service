using Ticketing.Application.Model.DTOs;
using Ticketing.Domain.Constants;
using Ticketing.Domain.Domain.EventSeatInventory.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.EventSeatInventory.Request;
using Ticketing.Infrastructure.DTOs.EventSeatInventory.Response;
using Ticketing.Infrastructure.Entities;
using Ticketing.Infrastructure.Entities.EventSeatInventory.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Repositories.EventSeatInventory;

namespace Ticketing.Domain.Domain.EventSeatInventory;

public class EventSeatInventoryDomainService(IEventSeatInventoryUnitOfWork unitOfWork)
    : BaseService<IEventSeatInventoryRepository, EventSeatInventoryEntity>(
        unitOfWork.EventSeatInventory,
        TicketingTypeEnum.EventSeatInventory), IEventSeatInventoryDomainService
{
    public async Task<ResponseMessage<int>> InsertAsync(EventSeatInventoryEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.InsertAsync(new
            {
                event_id = entity.event_id,
                seat_id = entity.seat_id,
                event_zone_id = entity.event_zone_id,
                seat_status = entity.seat_status,
                current_hold_id = entity.current_hold_id,
                current_order_item_id = entity.current_order_item_id,
                base_price = entity.base_price,
                version = entity.version
            }, cancellationToken)!.ToIntAsync();

            if (result is not > 0)
                throw new Exception(DomainMessageConstants.EventSeatInventory.InsertError);

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return new ResponseMessage<int>().MessageSuccess(result ?? 0, DomainMessageConstants.EventSeatInventory.InsertSuccess);
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<int>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> UpdateAsync(EventSeatInventoryEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.UpdateAsync(new
            {
                event_seat_inventory_id = entity.event_seat_inventory_id,
                event_id = entity.event_id,
                seat_id = entity.seat_id,
                event_zone_id = entity.event_zone_id,
                seat_status = entity.seat_status,
                current_hold_id = entity.current_hold_id,
                current_order_item_id = entity.current_order_item_id,
                base_price = entity.base_price,
                version = entity.version
            }, cancellationToken)!.ToBoolAsync();

            if (!result)
                throw new Exception(DomainMessageConstants.EventSeatInventory.UpdateError);

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return new ResponseMessage<bool>().MessageSuccess(true, DomainMessageConstants.EventSeatInventory.UpdateSuccess);
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> DeleteAsync(EventSeatInventoryEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.DeleteAsync(new
            {
                event_seat_inventory_id = entity.event_seat_inventory_id
            }, cancellationToken)!.ToBoolAsync();

            if (!result)
                throw new Exception(DomainMessageConstants.EventSeatInventory.DeleteError);

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return new ResponseMessage<bool>().MessageSuccess(true, DomainMessageConstants.EventSeatInventory.DeleteSuccess);
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<EventSeatInventoryDetailDto?>> GetByIdAsync(
        EventSeatInventoryGetByIdRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetAsync<EventSeatInventoryDetailDto>(new
            {
                event_seat_inventory_id = request.event_seat_inventory_id
            }, cancellationToken);

            if (result is null)
                return new ResponseMessage<EventSeatInventoryDetailDto?>().MessageWarning(DomainMessageConstants.EventSeatInventory.NotFound);

            return new ResponseMessage<EventSeatInventoryDetailDto?>().MessageSuccess(result, DomainMessageConstants.EventSeatInventory.GetDetailSuccess);
        }
        catch (Exception e)
        {
            return new ResponseMessage<EventSeatInventoryDetailDto?>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<EventSeatInventoryListDto>>> GetPagedListAsync(
        EventSeatInventoryGetPagedListRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetPagedAsync<EventSeatInventoryListDto>(new
            {
                pagesize = request.pagesize,
                offset = request.offset,
                keysearch = request.keysearch,
                event_id = request.event_id,
                event_zone_id = request.event_zone_id,
                seat_status = request.seat_status
            }, cancellationToken);

            return new ResponseMessage<IEnumerable<EventSeatInventoryListDto>>().MessageSuccess(result ?? [], DomainMessageConstants.EventSeatInventory.GetListSuccess);
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<EventSeatInventoryListDto>>().MessageError(e.Message);
        }
    }
}