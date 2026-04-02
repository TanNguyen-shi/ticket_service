using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.EventZone.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Persistence.Helpers;

namespace Ticketing.Infrastructure.Repositories.EventZone;

public interface IEventZoneRepository : IGenericRepository<EventZoneEntity>
{
    Task<IEnumerable<TResult>> GetByEventId<TResult>(object param, CancellationToken cancellationToken = default);
}

public class EventZoneRepository(
    IDapperProcedureHelper dapper,
    DapperContextAccessor contextAccessor)
    : Repository<EventZoneEntity>(dapper, contextAccessor), IEventZoneRepository
{
    protected override string Schema => "ticketing";
    protected override string TableName => "event_zone";

    public async Task<IEnumerable<TResult>> GetByEventId<TResult>(object param, CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("getbyeventid");
        return await _dapper.GetAllAsync<TResult>(
            Connection,
            spName,
            param?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }
}