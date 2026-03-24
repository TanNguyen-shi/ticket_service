using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.Ticket.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Persistence.Helpers;

namespace Ticketing.Infrastructure.Repositories.Ticket;

public interface ITicketRepository : IGenericRepository<TicketEntity>
{
}

public class TicketRepository(
    IDapperProcedureHelper dapper,
    DapperContextAccessor contextAccessor)
    : Repository<TicketEntity>(dapper, contextAccessor), ITicketRepository
{
    protected override string Schema => "ticketing";
    protected override string TableName => "ticket";

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

        return result?.ticket_id > 0 ? result.ticket_id.ToString() : null;
    }

    private class CommandResult
    {
        public long ticket_id { get; set; }
    }
}

