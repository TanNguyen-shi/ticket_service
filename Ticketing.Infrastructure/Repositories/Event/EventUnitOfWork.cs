using Ticketing.Infrastructure.Configurations;

namespace Ticketing.Infrastructure.Repositories.Event;

public interface IEventUnitOfWork : IUnitOfWork
{
    IEventRepository Event { get; set; }
}

public class EventUnitOfWork(
    IEventRepository eventRepository,
    DapperContext dapperContext,
    DapperContextAccessor contextAccessor)
    : UnitOfWork(dapperContext, contextAccessor), IEventUnitOfWork
{
    public IEventRepository Event { get; set; } =
        eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
}

