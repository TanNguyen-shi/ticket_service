using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.Event.Response;
using Ticketing.Infrastructure.Persistence.Helpers;

namespace Ticketing.Infrastructure.Repositories.Event;

public interface IEventRepository : IGenericRepository<EventEntity>
{
}

public class EventRepository(
    IDapperProcedureHelper dapper,
    DapperContextAccessor contextAccessor)
    : Repository<EventEntity>(dapper, contextAccessor), IEventRepository
{
    protected override string Schema => "ticketing";
    protected override string TableName => "event";
}

