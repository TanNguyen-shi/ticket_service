using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.DTOs.Client.Event.Request;
using Ticketing.Infrastructure.DTOs.Client.Event.Response;
using Ticketing.Infrastructure.DTOs.Event.Request;
using Ticketing.Infrastructure.DTOs.Event.Response;
using Ticketing.Infrastructure.Entities.Event.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Persistence.Helpers;

namespace Ticketing.Infrastructure.Repositories.Event;

public interface IEventRepository : IGenericRepository<EventEntity>
{
    // Client-side methods
    Task<IEnumerable<EventClientListDto>> GetFeaturedAsync(EventGetFeaturedClientRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<EventClientListDto>> GetTrendingAsync(EventGetTrendingClientRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<EventClientListDto>> GetUpcomingAsync(EventGetUpcomingClientRequest request, CancellationToken cancellationToken = default);
    Task<IEnumerable<EventClientListDto>> SearchAsync(EventSearchClientRequest request, CancellationToken cancellationToken = default);
}

public class EventRepository(
    IDapperProcedureHelper dapper,
    DapperContextAccessor contextAccessor)
    : Repository<EventEntity>(dapper, contextAccessor), IEventRepository
{
    protected override string Schema => "ticketing";
    protected override string TableName => "event";

    // ...existing code...

    public async Task<IEnumerable<EventClientListDto>> GetFeaturedAsync(
        EventGetFeaturedClientRequest request, 
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("get_featured_client");
        return await _dapper.GetAllAsync<EventClientListDto>(
            Connection,
            spName,
            request.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }

    public async Task<IEnumerable<EventClientListDto>> GetTrendingAsync(
        EventGetTrendingClientRequest request, 
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("get_trending_client");
        var parameters = new object[] { "p_limit", request.limit };
        
        return await _dapper.GetAllAsync<EventClientListDto>(
            Connection,
            spName,
            request.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }

    public async Task<IEnumerable<EventClientListDto>> GetUpcomingAsync(
        EventGetUpcomingClientRequest request, 
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("get_upcoming_client");
        
        return await _dapper.GetAllAsync<EventClientListDto>(
            Connection,
            spName,
            request.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }

    public async Task<IEnumerable<EventClientListDto>> SearchAsync(
        EventSearchClientRequest request,
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("search_client");
        return await _dapper.GetAllAsync<EventClientListDto>(
            Connection,
            spName,
            request.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }
}

