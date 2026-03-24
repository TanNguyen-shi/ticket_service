using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.EventSeatInventory.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Persistence.Helpers;

namespace Ticketing.Infrastructure.Repositories.EventSeatInventory;

public interface IEventSeatInventoryRepository : IGenericRepository<EventSeatInventoryEntity>
{
}

public class EventSeatInventoryRepository(
    IDapperProcedureHelper dapper,
    DapperContextAccessor contextAccessor)
    : Repository<EventSeatInventoryEntity>(dapper, contextAccessor), IEventSeatInventoryRepository
{
    protected override string Schema => "ticketing";
    protected override string TableName => "event_seat_inventory";

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
        var result = await _dapper.GetAsync<EventSeatInventoryCommandResult>(
            Connection,
            functionName,
            dto?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);

        return result?.event_seat_inventory_id > 0 ? result.event_seat_inventory_id.ToString() : null;
    }

    private class EventSeatInventoryCommandResult
    {
        public long event_seat_inventory_id { get; set; }
    }
}

