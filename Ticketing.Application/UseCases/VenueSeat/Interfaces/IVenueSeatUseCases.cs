using Ticketing.Application.Model.DTOs;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.VenueSeat.Request;
using Ticketing.Infrastructure.DTOs.VenueSeat.Response;

namespace Ticketing.Application.UseCases.VenueSeat.Interfaces;

public interface IVenueSeatUseCases
{
    Task<ResponseMessage<int>?> InsertAsync(VenueSeatCreateRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> UpdateAsync(VenueSeatUpdateRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> DeleteAsync(VenueSeatDeleteRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<VenueSeatDetailDto?>> GetByIdAsync(VenueSeatGetByIdRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<IEnumerable<VenueSeatListDto>>> GetPagedListAsync(VenueSeatGetPagedListRequest request, long? userLogin, CancellationToken cancellationToken = default);
}

