using Ticketing.Application.Model.DTOs;
using Ticketing.Application.UseCases.Event.Interfaces;
using Ticketing.Domain.Domain.Event.Interfaces;
using Ticketing.Infrastructure.DTOs.Client.Event.Request;
using Ticketing.Infrastructure.DTOs.Client.Event.Response;
using Ticketing.Infrastructure.DTOs.Event.Request;
using Ticketing.Infrastructure.DTOs.Event.Response;

namespace Ticketing.Application.UseCases.Event;

/// <summary>
/// Client-side Event Use Cases Implementation
/// Orchestrate giữa Controller và Domain Service
/// </summary>
public class EventClientUseCases(IEventDomainService eventDomain) : IEventClientUseCases
{
    public async Task<ResponseMessage<IEnumerable<EventClientListDto>>> GetFeaturedAsync(EventGetFeaturedClientRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            return await eventDomain.GetFeaturedAsync(request, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<EventClientListDto>>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<EventClientListDto>>> GetTrendingAsync(EventGetTrendingClientRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            return await eventDomain.GetTrendingAsync(request, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<EventClientListDto>>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<EventClientListDto>>> GetUpcomingAsync(EventGetUpcomingClientRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            return await eventDomain.GetUpcomingAsync(request, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<EventClientListDto>>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<EventClientListDto>>> SearchAsync(EventSearchClientRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            return await eventDomain.SearchAsync(request, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<EventClientListDto>>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<EventClientDetailDto?>> GetDetailEvent(EventGetByIdRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            return await eventDomain.GetEventDetail(request, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<EventClientDetailDto?>().MessageError(e.Message);
        }
    }
}