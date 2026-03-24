using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.TicketOrderItem.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Persistence.Helpers;

namespace Ticketing.Infrastructure.Repositories.TicketOrderItem;

public interface ITicketOrderItemRepository : IGenericRepository<TicketOrderItemEntity>
{
}

public class TicketOrderItemRepository(
    IDapperProcedureHelper dapper,
    DapperContextAccessor contextAccessor)
    : Repository<TicketOrderItemEntity>(dapper, contextAccessor), ITicketOrderItemRepository
{
    protected override string Schema => "ticketing";
    protected override string TableName => "ticket_order_item";

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
        var result = await _dapper.GetAsync<CommandResult>(
            Connection,
            functionName,
            dto?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);

        return result?.order_item_id > 0 ? result.order_item_id.ToString() : null;
    }

    private class CommandResult
    {
        public long order_item_id { get; set; }
    }
}

