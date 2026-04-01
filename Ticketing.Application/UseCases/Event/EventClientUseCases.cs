using Ticketing.Application.Model.DTOs;
using Ticketing.Application.UseCases.Event.Interfaces;
using Ticketing.Domain.Domain.Event.Interfaces;
using Ticketing.Infrastructure.DTOs.Event.Request;
using Ticketing.Infrastructure.DTOs.Event.Response;

namespace Ticketing.Application.UseCases.Event;

/// <summary>
/// Client-side Event Use Cases Implementation
/// Orchestrate giữa Controller và Domain Service
/// </summary>
public class EventClientUseCases(IEventDomainService eventDomain) : IEventClientUseCases
{
    public async Task<ResponseMessage<IEnumerable<EventClientDto>>> GetFeaturedAsync(EventGetFeaturedClientRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            return await eventDomain.GetFeaturedAsync(request, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<EventClientDto>>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<EventClientDto>>> GetTrendingAsync(EventGetTrendingClientRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            return await eventDomain.GetTrendingAsync(request, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<EventClientDto>>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<EventClientDto>>> GetUpcomingAsync(EventGetUpcomingClientRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            return await eventDomain.GetUpcomingAsync(request, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<EventClientDto>>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<EventClientDto>>> SearchAsync(EventSearchClientRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            return await eventDomain.SearchAsync(request, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<EventClientDto>>().MessageError(e.Message);
        }
    }
}

