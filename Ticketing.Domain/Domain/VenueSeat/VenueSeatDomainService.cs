using Ticketing.Application.Model.DTOs;
using Ticketing.Domain.Constants;
using Ticketing.Domain.Domain.VenueSeat.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.VenueSeat.Request;
using Ticketing.Infrastructure.DTOs.VenueSeat.Response;
using Ticketing.Infrastructure.Entities;
using Ticketing.Infrastructure.Entities.VenueSeat.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Repositories.VenueSeat;

namespace Ticketing.Domain.Domain.VenueSeat;

public class VenueSeatDomainService(IVenueSeatUnitOfWork unitOfWork)
    : BaseService<IVenueSeatRepository, VenueSeatEntity>(unitOfWork.VenueSeat, TicketingTypeEnum.VenueSeat),
        IVenueSeatDomainService
{
    public async Task<ResponseMessage<int>> InsertAsync(VenueSeatEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var isExisted = await _repository.CheckExistAsync(new
            {
                seat_id = 0,
                venue_id = entity.venue_id,
                seat_code = entity.seat_code
            }, cancellationToken)!.ToBoolAsync();

            if (isExisted)
            {
                await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
                return new ResponseMessage<int>().MessageWarning("Mã ghế đã tồn tại trong địa điểm");
            }

            var result = await _repository.InsertAsync(new
            {
                venue_id = entity.venue_id,
                section_id = entity.section_id,
                seat_code = entity.seat_code,
                row_label = entity.row_label,
                seat_number = entity.seat_number,
                seat_label = entity.seat_label,
                x_pos = entity.x_pos,
                y_pos = entity.y_pos,
                seat_type = entity.seat_type,
                status = entity.status,
                created_by = entity.created_by
            }, cancellationToken)!.ToIntAsync();

            if (result is not > 0)
                throw new Exception(DomainMessageConstants.VenueSeat.InsertError);

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return new ResponseMessage<int>().MessageSuccess(result ?? 0, DomainMessageConstants.VenueSeat.InsertSuccess);
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<int>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> UpdateAsync(VenueSeatEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var isExisted = await _repository.CheckExistAsync(new
            {
                seat_id = entity.seat_id,
                venue_id = entity.venue_id,
                seat_code = entity.seat_code
            }, cancellationToken)!.ToBoolAsync();

            if (isExisted)
            {
                await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
                return new ResponseMessage<bool>().MessageWarning("Mã ghế đã tồn tại trong địa điểm");
            }

            var result = await _repository.UpdateAsync(new
            {
                seat_id = entity.seat_id,
                venue_id = entity.venue_id,
                section_id = entity.section_id,
                seat_code = entity.seat_code,
                row_label = entity.row_label,
                seat_number = entity.seat_number,
                seat_label = entity.seat_label,
                x_pos = entity.x_pos,
                y_pos = entity.y_pos,
                seat_type = entity.seat_type,
                status = entity.status,
                updated_by = entity.updated_by
            }, cancellationToken)!.ToBoolAsync();

            if (!result)
                throw new Exception(DomainMessageConstants.VenueSeat.UpdateError);

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return new ResponseMessage<bool>().MessageSuccess(true, DomainMessageConstants.VenueSeat.UpdateSuccess);
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> DeleteAsync(VenueSeatEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.DeleteAsync(new
            {
                seat_id = entity.seat_id,
                deleted_by = entity.deleted_by
            }, cancellationToken)!.ToBoolAsync();

            if (!result)
                throw new Exception(DomainMessageConstants.VenueSeat.DeleteError);

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return new ResponseMessage<bool>().MessageSuccess(true, DomainMessageConstants.VenueSeat.DeleteSuccess);
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<VenueSeatDetailDto?>> GetByIdAsync(
        VenueSeatGetByIdRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetAsync<VenueSeatDetailDto>(new
            {
                seat_id = request.seat_id
            }, cancellationToken);

            if (result is null)
                return new ResponseMessage<VenueSeatDetailDto?>().MessageWarning(DomainMessageConstants.VenueSeat.NotFound);

            return new ResponseMessage<VenueSeatDetailDto?>().MessageSuccess(result, DomainMessageConstants.VenueSeat.GetDetailSuccess);
        }
        catch (Exception e)
        {
            return new ResponseMessage<VenueSeatDetailDto?>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<VenueSeatListDto>>> GetPagedListAsync(
        VenueSeatGetPagedListRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetPagedAsync<VenueSeatListDto>(new
            {
                pagesize = request.pagesize,
                offset = request.offset,
                venue_id = request.venue_id,
                section_id = request.section_id,
                keysearch = request.keysearch,
                status = request.status,
                seat_type = request.seat_type
            }, cancellationToken);

            return new ResponseMessage<IEnumerable<VenueSeatListDto>>().MessageSuccess(result ?? [], DomainMessageConstants.VenueSeat.GetListSuccess);
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<VenueSeatListDto>>().MessageError(e.Message);
        }
    }
}

