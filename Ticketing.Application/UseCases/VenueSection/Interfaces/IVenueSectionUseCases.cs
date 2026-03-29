using Ticketing.Application.Model.DTOs;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.VenueSection.Request;
using Ticketing.Infrastructure.DTOs.VenueSection.Response;

namespace Ticketing.Application.UseCases.VenueSection.Interfaces;

public interface IVenueSectionUseCases
{
    Task<ResponseMessage<int>?> InsertAsync(VenueSectionCreateRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> UpdateAsync(VenueSectionUpdateRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> DeleteAsync(VenueSectionDeleteRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<VenueSectionDetailDto?>> GetByIdAsync(VenueSectionGetByIdRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<IEnumerable<VenueSectionListDto>>> GetPagedListAsync(VenueSectionGetPagedListRequest request, long? userLogin, CancellationToken cancellationToken = default);
    Task<ResponseMessage<IEnumerable<VenueSectionListDto>>> GetAllAsync(long? userLogin, CancellationToken cancellationToken = default);
}