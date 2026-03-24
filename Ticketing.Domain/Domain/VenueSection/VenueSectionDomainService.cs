using Ticketing.Domain.Domain.VenueSection.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.VenueSection.Request;
using Ticketing.Infrastructure.DTOs.VenueSection.Response;
using Ticketing.Infrastructure.Entities;
using Ticketing.Infrastructure.Entities.VenueSection.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Repositories.VenueSection;

namespace Ticketing.Domain.Domain.VenueSection;

public class VenueSectionDomainService(IVenueSectionUnitOfWork unitOfWork)
    : BaseService<IVenueSectionRepository, VenueSectionEntity>(
        unitOfWork.VenueSection,
        TicketingTypeEnum.VenueSection), IVenueSectionDomainService
{
    public async Task<ResponseMessage<int>> InsertAsync(VenueSectionEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var isExisted = await _repository.CheckExistAsync(new
            {
                section_id = 0,
                venue_id = entity.venue_id,
                section_code = entity.section_code
            }, cancellationToken)!.ToBoolAsync();

            if (isExisted)
            {
                await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
                return new ResponseMessage<int>().MessageWarning("Mã khu vực đã tồn tại trong địa điểm");
            }

            var result = await _repository.InsertAsync(new
            {
                venue_id = entity.venue_id,
                section_code = entity.section_code,
                section_name = entity.section_name,
                display_order = entity.display_order,
                status = entity.status,
                created_by = entity.created_by
            }, cancellationToken)!.ToIntAsync();

            if (result is not > 0)
                throw new Exception("Thêm mới khu vực thất bại");

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return new ResponseMessage<int>().MessageSuccess(result ?? 0, "Thành công");
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<int>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> UpdateAsync(VenueSectionEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var isExisted = await _repository.CheckExistAsync(new
            {
                section_id = entity.section_id,
                venue_id = entity.venue_id,
                section_code = entity.section_code
            }, cancellationToken)!.ToBoolAsync();

            if (isExisted)
            {
                await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
                return new ResponseMessage<bool>().MessageWarning("Mã khu vực đã tồn tại trong địa điểm");
            }

            var result = await _repository.UpdateAsync(new
            {
                section_id = entity.section_id,
                venue_id = entity.venue_id,
                section_code = entity.section_code,
                section_name = entity.section_name,
                display_order = entity.display_order,
                status = entity.status,
                updated_by = entity.updated_by
            }, cancellationToken)!.ToBoolAsync();

            if (!result)
                throw new Exception("Cập nhật khu vực thất bại");

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return new ResponseMessage<bool>().MessageSuccess(true, "Thành công");
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> DeleteAsync(VenueSectionEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.DeleteAsync(new
            {
                section_id = entity.section_id,
                deleted_by = entity.deleted_by
            }, cancellationToken)!.ToBoolAsync();

            if (!result)
                throw new Exception("Xóa dữ liệu khu vực thất bại");

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return new ResponseMessage<bool>().MessageSuccess(true, "Thành công");
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<VenueSectionDetailDto?>> GetByIdAsync(
        VenueSectionGetByIdRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetAsync<VenueSectionDetailDto>(new
            {
                section_id = request.section_id
            }, cancellationToken);

            if (result is null)
                return new ResponseMessage<VenueSectionDetailDto?>().MessageWarning("Không tìm thấy dữ liệu");

            return new ResponseMessage<VenueSectionDetailDto?>().MessageSuccess(result, "Thành công");
        }
        catch (Exception e)
        {
            return new ResponseMessage<VenueSectionDetailDto?>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<VenueSectionListDto>>> GetPagedListAsync(
        VenueSectionGetPagedListRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetPagedAsync<VenueSectionListDto>(new
            {
                pagesize = request.pagesize,
                offset = request.offset,
                venue_id = request.venue_id,
                keysearch = request.keysearch,
                status = request.status
            }, cancellationToken);

            return new ResponseMessage<IEnumerable<VenueSectionListDto>>().MessageSuccess(result ?? [], "Thành công");
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<VenueSectionListDto>>().MessageError(e.Message);
        }
    }
}

