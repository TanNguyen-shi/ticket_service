using Ticketing.Application.Model.DTOs;
using Ticketing.Application.UseCases.Venue.Interfaces;
using Ticketing.Domain.Domain.Venue.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.Venue.Request;
using Ticketing.Infrastructure.DTOs.Venue.Response;
using Ticketing.Infrastructure.Entities;
using Ticketing.Infrastructure.Entities.Venue.Request;
using Ticketing.Infrastructure.Entities.Venue.Response;

namespace Ticketing.Application.UseCases.Venue;

public class VenueUseCases(IVenueDomainService venueDomain) : IVenueUseCases
{
    public async Task<ResponseMessage<int>?> InsertAsync(VenueCreateRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<int>();

        try
        {
            var insert = await venueDomain.InsertAsync(new VenueEntity
            {
                venue_code = request.venue_code,
                venue_name = request.venue_name,
                address_line = request.address_line,
                city = request.city,
                country = request.country,
                status = request.status,
                created_by = userLogin
            }, cancellationToken);

            if (!insert.issuccess)
                return new ResponseMessage<int>().MessageError(insert.message ?? "Cập nhật địa điểm thất bại");

            return new ResponseMessage<int>().MessageSuccess(insert.data, "Thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> UpdateAsync(VenueUpdateRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<bool>();

        try
        {
            var update = await venueDomain.UpdateAsync(new VenueEntity
            {
                venue_id = request.venue_id,
                venue_code = request.venue_code,
                venue_name = request.venue_name,
                address_line = request.address_line,
                city = request.city,
                country = request.country,
                status = request.status,
                updated_by = userLogin
            }, cancellationToken);

            if (!update.issuccess)
                return new ResponseMessage<bool>().MessageError(update.message ?? "Cập nhật địa điểm thất bại");

            return new ResponseMessage<bool>().MessageSuccess(true, "Thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> DeleteAsync(VenueDeleteRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<bool>();

        try
        {
            var delete = await venueDomain.DeleteAsync(new VenueEntity
            {
                venue_id = request.venue_id,
                deleted_by = userLogin
            }, cancellationToken);

            if (!delete.issuccess)
                return new ResponseMessage<bool>().MessageError(delete.message ?? "Cập nhật địa điểm thất bại");

            return new ResponseMessage<bool>().MessageSuccess(true, "Thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<VenueDetailDto?>> GetByIdAsync(VenueGetByIdRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        try
        {
            return await venueDomain.GetByIdAsync(new VenueGetByIdRequest
            {
                venue_id = request.venue_id
            }, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<VenueDetailDto?>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<VenueListDto>>> GetPagedListAsync(VenueGetPagedListRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        try
        {
            return await venueDomain.GetPagedListAsync(new VenueGetPagedListRequest
            {
                pagesize = request.pagesize,
                offset = request.offset,
                keysearch = request.keysearch,
                status = request.status,
                city = request.city
            }, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<VenueListDto>>().MessageError(e.Message);
        }
    }
}