using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.Venue.Request;
using Ticketing.Infrastructure.DTOs.Venue.Response;
using Ticketing.Infrastructure.Entities;
using Ticketing.Infrastructure.Entities.Venue.Request;
using Ticketing.Infrastructure.Entities.Venue.Response;

namespace Ticketing.Domain.Domain.Venue.Interfaces;

public interface IVenueDomainService
{
    Task<ResponseMessage<int>> InsertAsync(VenueEntity entity, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> UpdateAsync(VenueEntity entity, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> DeleteAsync(VenueEntity entity, CancellationToken cancellationToken = default);

    Task<ResponseMessage<VenueDetailDto?>> GetByIdAsync(VenueGetByIdRequest request, CancellationToken cancellationToken = default);

    Task<ResponseMessage<IEnumerable<VenueListDto>>> GetPagedListAsync(VenueGetPagedListRequest request, CancellationToken cancellationToken = default);
}