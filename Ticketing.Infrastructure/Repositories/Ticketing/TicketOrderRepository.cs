using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.TicketOrder.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Persistence.Helpers;

namespace Ticketing.Infrastructure.Repositories.TicketOrder;

public interface ITicketOrderRepository : IGenericRepository<TicketOrderEntity>
{
}

public class TicketOrderRepository(
    IDapperProcedureHelper dapper,
    DapperContextAccessor contextAccessor)
    : Repository<TicketOrderEntity>(dapper, contextAccessor), ITicketOrderRepository
{
    protected override string Schema => "ticketing";
    protected override string TableName => "ticket_order";
}