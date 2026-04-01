using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.DTOs.Event.Request;
using Ticketing.Infrastructure.DTOs.Event.Response;
using Ticketing.Infrastructure.Entities.Event.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Persistence.Helpers;

namespace Ticketing.Infrastructure.Repositories.Event;

public interface IEventRepository : IGenericRepository<EventEntity>
{
    // Client-side methods
    Task<IEnumerable<EventClientDto>> GetFeaturedAsync(EventGetFeaturedClientRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<EventClientDto>> GetTrendingAsync(EventGetTrendingClientRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<EventClientDto>> GetUpcomingAsync(EventGetUpcomingClientRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<EventClientDto>> SearchAsync(EventSearchClientRequest request, CancellationToken cancellationToken = default);
}

public class EventRepository(
    IDapperProcedureHelper dapper,
    DapperContextAccessor contextAccessor)
    : Repository<EventEntity>(dapper, contextAccessor), IEventRepository
{
    protected override string Schema => "ticketing";
    protected override string TableName => "event";

    // ...existing code...

    public async Task<IEnumerable<EventClientDto>> GetFeaturedAsync(
        EventGetFeaturedClientRequest request, 
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("get_featured_client");
        return await _dapper.GetAllAsync<EventClientDto>(
            Connection,
            spName,
            request.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }

    public async Task<IEnumerable<EventClientDto>> GetTrendingAsync(
        EventGetTrendingClientRequest request, 
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("get_trending_client");
        var parameters = new object[] { "p_limit", request.limit };
        
        return await _dapper.GetAllAsync<EventClientDto>(
            Connection,
            spName,
            request.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }

    public async Task<IEnumerable<EventClientDto>> GetUpcomingAsync(
        EventGetUpcomingClientRequest request, 
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("get_upcoming_client");
        
        return await _dapper.GetAllAsync<EventClientDto>(
            Connection,
            spName,
            request.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }

    public async Task<IEnumerable<EventClientDto>> SearchAsync(
        EventSearchClientRequest request,
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("search_client");
        return await _dapper.GetAllAsync<EventClientDto>(
            Connection,
            spName,
            request.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }
}

