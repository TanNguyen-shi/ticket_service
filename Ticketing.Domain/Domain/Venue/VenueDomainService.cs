using Ticketing.Application.Model.DTOs;
using Ticketing.Domain.Domain.Venue.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.Admin.Venue.Response;
using Ticketing.Infrastructure.DTOs.Venue.Request;
using Ticketing.Infrastructure.DTOs.Venue.Response;
using Ticketing.Infrastructure.Entities;
using Ticketing.Infrastructure.Entities.Venue;
using Ticketing.Infrastructure.Entities.Venue.Request;
using Ticketing.Infrastructure.Entities.Venue.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Repositories.Venue;

namespace Ticketing.Domain.Domain.Venue;

public class VenueDomainService(IVenueUnitOfWork unitOfWork)
    : BaseService<IVenueRepository, VenueEntity>(unitOfWork.Venue, TicketingTypeEnum.Venue), IVenueDomainService
{
    public async Task<ResponseMessage<int>> InsertAsync(VenueEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.InsertAsync(new
            {
                venue_code = entity.venue_code,
                venue_name = entity.venue_name,
                address_line = entity.address_line,
                city = entity.city,
                country = entity.country,
                status = entity.status,
                created_by = entity.created_by
            }, cancellationToken)!.ToIntAsync();

            if (result is not > 0)
                throw new Exception("Thêm địa điểm thất bại");

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return new ResponseMessage<int>().MessageSuccess(result ?? 0, "Thêm địa điểm thành công");
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<int>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> UpdateAsync(VenueEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.UpdateAsync(new
            {
                venue_id = entity.venue_id,
                venue_code = entity.venue_code,
                venue_name = entity.venue_name,
                address_line = entity.address_line,
                city = entity.city,
                country = entity.country,
                status = entity.status,
                updated_by = entity.updated_by
            }, cancellationToken)!.ToBoolAsync();

            if (!result)
                throw new Exception("Cập nhật địa điểm thất bại");

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return new ResponseMessage<bool>().MessageSuccess(true, "Cập nhật địa điểm thành công");
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> DeleteAsync(VenueEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.DeleteAsync(new
            {
                venue_id = entity.venue_id,
                deleted_by = entity.deleted_by
            }, cancellationToken)!.ToBoolAsync();

            if (!result)
                throw new Exception("Xóa địa điểm thất bại");

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return new ResponseMessage<bool>().MessageSuccess(true, "Xóa địa điểm thành công");
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<VenueDetailDto?>> GetByIdAsync(VenueGetByIdRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetAsync<VenueDetailDto>(new
            {
                venue_id = request.venue_id
            }, cancellationToken);

            if (result is null)
                return new ResponseMessage<VenueDetailDto?>().MessageWarning("Không tìm thấy thông tin địa điểm");

            return new ResponseMessage<VenueDetailDto?>().MessageSuccess(result, "Lấy chi tiết địa điểm thành công");
        }
        catch (Exception e)
        {
            return new ResponseMessage<VenueDetailDto?>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<VenueListDto>>> GetPagedListAsync(VenueGetPagedListRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetPagedAsync<VenueListDto>(new
            {
                pagesize = request.pagesize,
                offset = request.offset,
                keysearch = request.keysearch,
                status = request.status,
                city = request.city
            }, cancellationToken);

            return new ResponseMessage<IEnumerable<VenueListDto>>().MessageSuccess(result ?? [], "Lấy danh sách địa điểm thành công");
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<VenueListDto>>().MessageError(e.Message);
        }
    }
    
    public async Task<ResponseMessage<IEnumerable<VenueListDto>>> GetAll(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetAllAsync<VenueListDto>(new
            {
            }, cancellationToken);

            return new ResponseMessage<IEnumerable<VenueListDto>>().MessageSuccess(result ?? [], "Lấy toàn bộ địa điểm thành công");
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<VenueListDto>>().MessageError(e.Message);
        }
    }
    
    
    
}