using Ticketing.Application.Model.DTOs;
using Ticketing.Domain.Constants;
using Ticketing.Domain.Domain.TicketOrder.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.TicketOrder.Request;
using Ticketing.Infrastructure.DTOs.TicketOrder.Response;
using Ticketing.Infrastructure.Entities;
using Ticketing.Infrastructure.Entities.TicketOrder.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Repositories.TicketOrder;
using Ticketing.Infrastructure.Repositories.Ticketing;

namespace Ticketing.Domain.Domain.TicketOrder;

public class TicketOrderDomainService(ITicketingUnitOfWork unitOfWork)
    : BaseService<ITicketOrderRepository, TicketOrderEntity>(unitOfWork.TicketOrder, TicketingTypeEnum.TicketOrder), ITicketOrderDomainService
{
    public async Task<ResponseMessage<int>> InsertAsync(TicketOrderEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);
        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

             var result = await _repository.InsertAsync(new
             {
                 order_code = entity.order_code,
                 event_id = entity.event_id,
                 customer_id = entity.customer_id,
                 hold_id = entity.hold_id,
                 total_amount = entity.total_amount,
                 discount_amount = entity.discount_amount,
                 final_amount = entity.final_amount,
                 order_status = entity.order_status,
                 expired_at = entity.expired_at,
                 paid_at = entity.paid_at
             }, cancellationToken)!.ToIntAsync();

            if (result is not > 0)
                throw new Exception(DomainMessageConstants.TicketOrder.InsertError);

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<int>().MessageSuccess(result ?? 0, DomainMessageConstants.TicketOrder.InsertSuccess);
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<int>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> UpdateAsync(TicketOrderEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);
        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

             var result = await _repository.UpdateAsync(new
             {
                 order_id = entity.order_id,
                 order_code = entity.order_code,
                 event_id = entity.event_id,
                 customer_id = entity.customer_id,
                 hold_id = entity.hold_id,
                 total_amount = entity.total_amount,
                 discount_amount = entity.discount_amount,
                 final_amount = entity.final_amount,
                 order_status = entity.order_status,
                 expired_at = entity.expired_at,
                 paid_at = entity.paid_at
             }, cancellationToken)!.ToBoolAsync();

            if (!result)
                throw new Exception(DomainMessageConstants.TicketOrder.UpdateError);

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageSuccess(true, DomainMessageConstants.TicketOrder.UpdateSuccess);
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> DeleteAsync(TicketOrderEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);
        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.DeleteAsync(new
            {
                order_id = entity.order_id
            }, cancellationToken)!.ToBoolAsync();

            if (!result)
                throw new Exception(DomainMessageConstants.TicketOrder.DeleteError);

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageSuccess(true, DomainMessageConstants.TicketOrder.DeleteSuccess);
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<TicketOrderDetailDto?>> GetByIdAsync(TicketOrderGetByIdRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetAsync<TicketOrderDetailDto>(new { order_id = request.order_id }, cancellationToken);
            if (result is null)
                return new ResponseMessage<TicketOrderDetailDto?>().MessageWarning(DomainMessageConstants.TicketOrder.NotFound);
            return new ResponseMessage<TicketOrderDetailDto?>().MessageSuccess(result, DomainMessageConstants.TicketOrder.GetDetailSuccess);
        }
        catch (Exception e)
        {
            return new ResponseMessage<TicketOrderDetailDto?>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<TicketOrderListDto>>> GetPagedListAsync(TicketOrderGetPagedListRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
             var result = await _repository.GetPagedAsync<TicketOrderListDto>(new
             {
                 pagesize = request.pagesize,
                 offset = request.offset,
                 keysearch = request.keysearch,
                 event_id = request.event_id,
                 customer_id = request.customer_id,
                 order_status = request.order_status
             }, cancellationToken);

            return new ResponseMessage<IEnumerable<TicketOrderListDto>>().MessageSuccess(result ?? [], DomainMessageConstants.TicketOrder.GetListSuccess);
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<TicketOrderListDto>>().MessageError(e.Message);
        }
    }
}
