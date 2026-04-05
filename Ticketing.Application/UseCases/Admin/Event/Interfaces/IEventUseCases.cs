using Ticketing.Application.Model.DTOs;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.Admin.Event.Request;
using Ticketing.Infrastructure.DTOs.Event.Request;
using Ticketing.Infrastructure.DTOs.Event.Response;

namespace Ticketing.Application.UseCases.Event.Interfaces;

public interface IEventUseCases
{
    Task<ResponseMessage<int>?> InsertAsync(EventCreateRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> UpdateAsync(EventUpdateRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<bool>> DeleteAsync(EventDeleteRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<EventDetailDto?>> GetByIdAsync(EventGetByIdRequest request, long? userLogin, CancellationToken cancellationToken = default);

    Task<ResponseMessage<IEnumerable<EventListDto>>> GetPagedListAsync(EventGetPagedListRequest request, long? userLogin, CancellationToken cancellationToken = default);
}

