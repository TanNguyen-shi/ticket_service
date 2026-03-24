using Ticketing.Domain.Domain.Ticket.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.Ticket.Request;
using Ticketing.Infrastructure.DTOs.Ticket.Response;
using Ticketing.Infrastructure.Entities;
using Ticketing.Infrastructure.Entities.Ticket.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Repositories.Ticket;
using Ticketing.Infrastructure.Repositories.Ticketing;

namespace Ticketing.Domain.Domain.Ticket;

public class TicketDomainService(ITicketingUnitOfWork unitOfWork)
    : BaseService<ITicketRepository, TicketEntity>(unitOfWork.Ticket, TicketingTypeEnum.Ticket), ITicketDomainService
{
    public async Task<ResponseMessage<int>> InsertAsync(TicketEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);
        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.InsertAsync(new
            {
                ticket_code = entity.ticket_code,
                order_item_id = entity.order_item_id,
                event_id = entity.event_id,
                user_id = entity.user_id,
                seat_id = entity.seat_id,
                seat_label_snapshot = entity.seat_label_snapshot,
                zone_name_snapshot = entity.zone_name_snapshot,
                event_name_snapshot = entity.event_name_snapshot,
                ticket_status = entity.ticket_status,
                issued_at = entity.issued_at,
                checked_in_at = entity.checked_in_at
            }, cancellationToken)!.ToIntAsync();

            if (result is not > 0)
                throw new Exception("Thêm mới vé thất bại");

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<int>().MessageSuccess(result ?? 0, "Thành công");
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<int>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> UpdateAsync(TicketEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);
        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.UpdateAsync(new
            {
                ticket_id = entity.ticket_id,
                ticket_code = entity.ticket_code,
                order_item_id = entity.order_item_id,
                event_id = entity.event_id,
                user_id = entity.user_id,
                seat_id = entity.seat_id,
                seat_label_snapshot = entity.seat_label_snapshot,
                zone_name_snapshot = entity.zone_name_snapshot,
                event_name_snapshot = entity.event_name_snapshot,
                ticket_status = entity.ticket_status,
                issued_at = entity.issued_at,
                checked_in_at = entity.checked_in_at
            }, cancellationToken)!.ToBoolAsync();

            if (!result)
                throw new Exception("Cập nhật vé thất bại");

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageSuccess(true, "Thành công");
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> DeleteAsync(TicketEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);
        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);
            var result = await _repository.DeleteAsync(new { ticket_id = entity.ticket_id }, cancellationToken)!.ToBoolAsync();

            if (!result)
                throw new Exception("Xóa vé thất bại");

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageSuccess(true, "Thành công");
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<TicketDetailDto?>> GetByIdAsync(TicketGetByIdRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetAsync<TicketDetailDto>(new { ticket_id = request.ticket_id }, cancellationToken);
            if (result is null)
                return new ResponseMessage<TicketDetailDto?>().MessageWarning("Không tìm thấy dữ liệu");
            return new ResponseMessage<TicketDetailDto?>().MessageSuccess(result, "Thành công");
        }
        catch (Exception e)
        {
            return new ResponseMessage<TicketDetailDto?>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<TicketListDto>>> GetPagedListAsync(TicketGetPagedListRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetPagedAsync<TicketListDto>(new
            {
                pagesize = request.pagesize,
                offset = request.offset,
                keysearch = request.keysearch,
                event_id = request.event_id,
                user_id = request.user_id,
                ticket_status = request.ticket_status
            }, cancellationToken);

            return new ResponseMessage<IEnumerable<TicketListDto>>().MessageSuccess(result ?? [], "Thành công");
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<TicketListDto>>().MessageError(e.Message);
        }
    }
}
