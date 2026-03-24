using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.EventSeatInventory.Response;
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
}

