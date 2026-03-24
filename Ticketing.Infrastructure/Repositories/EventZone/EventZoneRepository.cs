using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.EventZone.Response;
using Ticketing.Infrastructure.Persistence.Helpers;

namespace Ticketing.Infrastructure.Repositories.EventZone;

public interface IEventZoneRepository : IGenericRepository<EventZoneEntity>
{
}

public class EventZoneRepository(
    IDapperProcedureHelper dapper,
    DapperContextAccessor contextAccessor)
    : Repository<EventZoneEntity>(dapper, contextAccessor), IEventZoneRepository
{
    protected override string Schema => "ticketing";
    protected override string TableName => "event_zone";
}

