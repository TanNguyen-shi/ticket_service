using Ticketing.Application.Model.DTOs;
using Ticketing.Application.UseCases.VenueSection.Interfaces;
using Ticketing.Domain.Domain.VenueSection.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.VenueSection.Request;
using Ticketing.Infrastructure.DTOs.VenueSection.Response;
using Ticketing.Infrastructure.Entities.VenueSection.Response;

namespace Ticketing.Application.UseCases.VenueSection;

public class VenueSectionUseCases(IVenueSectionDomainService venueSectionDomain) : IVenueSectionUseCases
{
    public async Task<ResponseMessage<int>?> InsertAsync(
        VenueSectionCreateRequest request,
        long? userLogin,
        CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<int>();

        try
        {
            var insert = await venueSectionDomain.InsertAsync(new VenueSectionEntity
            {
                venue_id = request.venue_id,
                section_code = request.section_code,
                section_name = request.section_name,
                display_order = request.display_order,
                status = request.status,
                created_by = userLogin
            }, cancellationToken);

            if (!insert.issuccess)
                return new ResponseMessage<int>().MessageError(insert.message ?? "Thêm mới khu vực thất bại");

            return new ResponseMessage<int>().MessageSuccess(insert.data, "Thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> UpdateAsync(
        VenueSectionUpdateRequest request,
        long? userLogin,
        CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<bool>();

        try
        {
            var update = await venueSectionDomain.UpdateAsync(new VenueSectionEntity
            {
                section_id = request.section_id,
                venue_id = request.venue_id,
                section_code = request.section_code,
                section_name = request.section_name,
                display_order = request.display_order,
                status = request.status,
                updated_by = userLogin
            }, cancellationToken);

            if (!update.issuccess)
                return new ResponseMessage<bool>().MessageError(update.message ?? "Cập nhật khu vực thất bại");

            return new ResponseMessage<bool>().MessageSuccess(true, "Thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> DeleteAsync(
        VenueSectionDeleteRequest request,
        long? userLogin,
        CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<bool>();

        try
        {
            var delete = await venueSectionDomain.DeleteAsync(new VenueSectionEntity
            {
                section_id = request.section_id,
                deleted_by = userLogin
            }, cancellationToken);

            if (!delete.issuccess)
                return new ResponseMessage<bool>().MessageError(delete.message ?? "Xóa khu vực thất bại");

            return new ResponseMessage<bool>().MessageSuccess(true, "Thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<VenueSectionDetailDto?>> GetByIdAsync(
        VenueSectionGetByIdRequest request,
        long? userLogin,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await venueSectionDomain.GetByIdAsync(new VenueSectionGetByIdRequest
            {
                section_id = request.section_id
            }, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<VenueSectionDetailDto?>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<VenueSectionListDto>>> GetPagedListAsync(
        VenueSectionGetPagedListRequest request,
        long? userLogin,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await venueSectionDomain.GetPagedListAsync(new VenueSectionGetPagedListRequest
            {
                pagesize = request.pagesize,
                offset = request.offset,
                venue_id = request.venue_id,
                keysearch = request.keysearch,
                status = request.status
            }, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<VenueSectionListDto>>().MessageError(e.Message);
        }
    }
}

