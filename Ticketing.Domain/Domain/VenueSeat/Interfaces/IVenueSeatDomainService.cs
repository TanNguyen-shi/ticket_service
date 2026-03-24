using Ticketing.Application.Model.DTOs;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.VenueSeat.Request;
using Ticketing.Infrastructure.DTOs.VenueSeat.Response;
using Ticketing.Infrastructure.Entities.VenueSeat.Response;

namespace Ticketing.Domain.Domain.VenueSeat.Interfaces;

public interface IVenueSeatDomainService
{
    Task<ResponseMessage<int>> InsertAsync(VenueSeatEntity entity, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> UpdateAsync(VenueSeatEntity entity, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> DeleteAsync(VenueSeatEntity entity, CancellationToken cancellationToken = default);

    Task<ResponseMessage<VenueSeatDetailDto?>> GetByIdAsync(
        VenueSeatGetByIdRequest request,
        CancellationToken cancellationToken = default);

    Task<ResponseMessage<IEnumerable<VenueSeatListDto>>> GetPagedListAsync(
        VenueSeatGetPagedListRequest request,
        CancellationToken cancellationToken = default);
}

