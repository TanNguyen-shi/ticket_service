using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.Ticket.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Persistence.Helpers;

namespace Ticketing.Infrastructure.Repositories.Ticket;

public interface ITicketRepository : IGenericRepository<TicketEntity>
{
    Task<IEnumerable<TResult>> GetByCustomerIdAsync<TResult>(object param, CancellationToken cancellationToken = default);
}

public class TicketRepository(
    IDapperProcedureHelper dapper,
    DapperContextAccessor contextAccessor)
    : Repository<TicketEntity>(dapper, contextAccessor), ITicketRepository
{
    protected override string Schema => "ticketing";
    protected override string TableName => "ticket";

    public async Task<IEnumerable<TResult>> GetByCustomerIdAsync<TResult>(
        object param,
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("getbycustomerid");
        return await _dapper.GetAllAsync<TResult>(
            Connection,
            spName,
            param?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }
}

