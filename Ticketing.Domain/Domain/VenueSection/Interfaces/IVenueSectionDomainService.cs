using Ticketing.Application.Model.DTOs;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.VenueSection.Request;
using Ticketing.Infrastructure.DTOs.VenueSection.Response;
using Ticketing.Infrastructure.Entities.VenueSection.Response;

namespace Ticketing.Domain.Domain.VenueSection.Interfaces;

public interface IVenueSectionDomainService
{
    Task<ResponseMessage<int>> InsertAsync(VenueSectionEntity entity, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> UpdateAsync(VenueSectionEntity entity, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> DeleteAsync(VenueSectionEntity entity, CancellationToken cancellationToken = default);

    Task<ResponseMessage<VenueSectionDetailDto?>> GetByIdAsync(
        VenueSectionGetByIdRequest request,
        CancellationToken cancellationToken = default);

    Task<ResponseMessage<IEnumerable<VenueSectionListDto>>> GetPagedListAsync(
        VenueSectionGetPagedListRequest request,
        CancellationToken cancellationToken = default);

    Task<ResponseMessage<IEnumerable<VenueSectionListDto>>> GetAllAsync(CancellationToken cancellationToken = default);
}