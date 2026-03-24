using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.Ticket.Response;
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
}

