using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.EventPublishLog.Response;
using Ticketing.Infrastructure.Extensions;
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

    public override async Task<string?> InsertAsync<TParam>(TParam dto, CancellationToken cancellationToken = default)
    {
        return await ExecuteCommandReturnIdAsync(GetSpName("insert"), dto, cancellationToken);
    }

    public override async Task<string?> UpdateAsync<TParam>(TParam dto, CancellationToken cancellationToken = default)
    {
        return await ExecuteCommandReturnIdAsync(GetSpName("update"), dto, cancellationToken);
    }

    public override async Task<string?> DeleteAsync<TParam>(TParam dto, CancellationToken cancellationToken = default)
    {
        return await ExecuteCommandReturnIdAsync(GetSpName("delete"), dto, cancellationToken);
    }

    private async Task<string?> ExecuteCommandReturnIdAsync<TParam>(
        string functionName,
        TParam dto,
        CancellationToken cancellationToken)
    {
        var result = await _dapper.GetAsync<EventPublishLogCommandResult>(
            Connection,
            functionName,
            dto?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);

        return result?.event_publish_log_id > 0 ? result.event_publish_log_id.ToString() : null;
    }

    private class EventPublishLogCommandResult
    {
        public long event_publish_log_id { get; set; }
    }
}

