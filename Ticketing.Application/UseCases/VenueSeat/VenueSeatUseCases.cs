using Ticketing.Application.Model.DTOs;
using Ticketing.Application.UseCases.VenueSeat.Interfaces;
using Ticketing.Domain.Domain.VenueSeat.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.VenueSeat.Request;
using Ticketing.Infrastructure.DTOs.VenueSeat.Response;
using Ticketing.Infrastructure.Entities.VenueSeat.Response;

namespace Ticketing.Application.UseCases.VenueSeat;

public class VenueSeatUseCases(IVenueSeatDomainService venueSeatDomain) : IVenueSeatUseCases
{
    public async Task<ResponseMessage<int>?> InsertAsync(
        VenueSeatCreateRequest request,
        long? userLogin,
        CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<int>();

        try
        {
            var insert = await venueSeatDomain.InsertAsync(new VenueSeatEntity
            {
                venue_id = request.venue_id,
                section_id = request.section_id,
                seat_code = request.seat_code,
                row_label = request.row_label,
                seat_number = request.seat_number,
                seat_label = request.seat_label,
                x_pos = request.x_pos,
                y_pos = request.y_pos,
                seat_type = request.seat_type,
                status = request.status,
                created_by = userLogin
            }, cancellationToken);

            if (!insert.issuccess)
                return new ResponseMessage<int>().MessageError(insert.message ?? "Thêm mới ghế thất bại");

            return new ResponseMessage<int>().MessageSuccess(insert.data, "Thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> UpdateAsync(
        VenueSeatUpdateRequest request,
        long? userLogin,
        CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<bool>();

        try
        {
            var update = await venueSeatDomain.UpdateAsync(new VenueSeatEntity
            {
                seat_id = request.seat_id,
                venue_id = request.venue_id,
                section_id = request.section_id,
                seat_code = request.seat_code,
                row_label = request.row_label,
                seat_number = request.seat_number,
                seat_label = request.seat_label,
                x_pos = request.x_pos,
                y_pos = request.y_pos,
                seat_type = request.seat_type,
                status = request.status,
                updated_by = userLogin
            }, cancellationToken);

            if (!update.issuccess)
                return new ResponseMessage<bool>().MessageError(update.message ?? "Cập nhật ghế thất bại");

            return new ResponseMessage<bool>().MessageSuccess(true, "Thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> DeleteAsync(
        VenueSeatDeleteRequest request,
        long? userLogin,
        CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<bool>();

        try
        {
            var delete = await venueSeatDomain.DeleteAsync(new VenueSeatEntity
            {
                seat_id = request.seat_id,
                deleted_by = userLogin
            }, cancellationToken);

            if (!delete.issuccess)
                return new ResponseMessage<bool>().MessageError(delete.message ?? "Xóa ghế thất bại");

            return new ResponseMessage<bool>().MessageSuccess(true, "Thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<VenueSeatDetailDto?>> GetByIdAsync(
        VenueSeatGetByIdRequest request,
        long? userLogin,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await venueSeatDomain.GetByIdAsync(new VenueSeatGetByIdRequest
            {
                seat_id = request.seat_id
            }, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<VenueSeatDetailDto?>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<VenueSeatListDto>>> GetPagedListAsync(
        VenueSeatGetPagedListRequest request,
        long? userLogin,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await venueSeatDomain.GetPagedListAsync(new VenueSeatGetPagedListRequest
            {
                pagesize = request.pagesize,
                offset = request.offset,
                venue_id = request.venue_id,
                section_id = request.section_id,
                keysearch = request.keysearch,
                status = request.status,
                seat_type = request.seat_type
            }, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<VenueSeatListDto>>().MessageError(e.Message);
        }
    }
}

