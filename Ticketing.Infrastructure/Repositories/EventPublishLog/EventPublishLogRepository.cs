using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.EventPublishLog.Response;
using Ticketing.Infrastructure.Persistence.Helpers;

namespace Ticketing.Infrastructure.Repositories.EventPublishLog;

public interface IEventPublishLogRepository : IGenericRepository<EventPublishLogEntity>
{
}

public class EventPublishLogRepository(
    IDapperProcedureHelper dapper,
    DapperContextAccessor contextAccessor)
    : Repository<EventPublishLogEntity>(dapper, contextAccessor), IEventPublishLogRepository
{
    protected override string Schema => "ticketing";
    protected override string TableName => "event_publish_log";
}

