using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.TicketOrderItem.Response;
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
}

