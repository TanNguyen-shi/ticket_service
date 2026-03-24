using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.Event.Response;
using Ticketing.Infrastructure.Extensions;
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
        var result = await _dapper.GetAsync<EventCommandResult>(
            Connection,
            functionName,
            dto?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);

        return result?.event_id > 0 ? result.event_id.ToString() : null;
    }

    private class EventCommandResult
    {
        public long event_id { get; set; }
    }
}

