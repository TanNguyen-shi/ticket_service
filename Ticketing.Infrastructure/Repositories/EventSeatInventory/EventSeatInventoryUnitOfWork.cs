using Ticketing.Infrastructure.Configurations;

namespace Ticketing.Infrastructure.Repositories.EventSeatInventory;

public interface IEventSeatInventoryUnitOfWork : IUnitOfWork
{
    IEventSeatInventoryRepository EventSeatInventory { get; set; }
}

public class EventSeatInventoryUnitOfWork(
    IEventSeatInventoryRepository eventSeatInventoryRepository,
    DapperContext dapperContext,
    DapperContextAccessor contextAccessor)
    : UnitOfWork(dapperContext, contextAccessor), IEventSeatInventoryUnitOfWork
{
    public IEventSeatInventoryRepository EventSeatInventory { get; set; } =
        eventSeatInventoryRepository ?? throw new ArgumentNullException(nameof(eventSeatInventoryRepository));
}

