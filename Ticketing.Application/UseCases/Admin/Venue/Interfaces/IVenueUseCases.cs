using Ticketing.Application.Model.DTOs;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.Admin.Venue.Response;
using Ticketing.Infrastructure.DTOs.Venue.Request;
using Ticketing.Infrastructure.DTOs.Venue.Response;
using Ticketing.Infrastructure.Entities.Venue.Response;

namespace Ticketing.Application.UseCases.Venue.Interfaces;

public interface IVenueUseCases
{
    Task<ResponseMessage<int>?> InsertAsync(VenueCreateRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> UpdateAsync(VenueUpdateRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> DeleteAsync(VenueDeleteRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<VenueDetailDto?>> GetByIdAsync(VenueGetByIdRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<IEnumerable<VenueListDto>>> GetPagedListAsync(VenueGetPagedListRequest request, long? userLogin, CancellationToken cancellationToken = default);
    Task<ResponseMessage<IEnumerable<VenueListDto>>> GetAllAsync(long? userLogin, CancellationToken cancellationToken = default);
}