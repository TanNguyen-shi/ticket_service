using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.EventZone.Response;
using Ticketing.Infrastructure.Extensions;
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
        var result = await _dapper.GetAsync<EventZoneCommandResult>(
            Connection,
            functionName,
            dto?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);

        return result?.event_zone_id > 0 ? result.event_zone_id.ToString() : null;
    }

    private class EventZoneCommandResult
    {
        public long event_zone_id { get; set; }
    }
}

