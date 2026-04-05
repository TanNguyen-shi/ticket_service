using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.EventZoneSection.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Persistence.Helpers;

namespace Ticketing.Infrastructure.Repositories.EventZoneSection;

public interface IEventZoneSectionRepository : IGenericRepository<EventZoneSectionEntity>
{
    Task<IEnumerable<TResult>> GetByEventZoneId<TResult>(object param, CancellationToken cancellationToken = default);
    Task<IEnumerable<TResult>> GetByEventId<TResult>(object param, CancellationToken cancellationToken = default);
}

public class EventZoneSectionRepository(
    IDapperProcedureHelper dapper,
    DapperContextAccessor contextAccessor)
    : Repository<EventZoneSectionEntity>(dapper, contextAccessor), IEventZoneSectionRepository
{
    protected override string Schema => "ticketing";
    protected override string TableName => "event_zone_section";


    public async Task<IEnumerable<TResult>> GetByEventZoneId<TResult>(object param, CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("getbyeventzoneid");
        return await _dapper.GetAllAsync<TResult>(
            Connection,
            spName,
            param?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }
    
    public async Task<IEnumerable<TResult>> GetByEventId<TResult>(object param, CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("geteventid");
        return await _dapper.GetAllAsync<TResult>(
            Connection,
            spName,
            param?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }
}