using Ticketing.Infrastructure.Configurations;

namespace Ticketing.Infrastructure.Repositories.EventZone;

public interface IEventZoneUnitOfWork : IUnitOfWork
{
    IEventZoneRepository EventZone { get; set; }
}

public class EventZoneUnitOfWork(
    IEventZoneRepository eventZoneRepository,
    DapperContext dapperContext,
    DapperContextAccessor contextAccessor)
    : UnitOfWork(dapperContext, contextAccessor), IEventZoneUnitOfWork
{
    public IEventZoneRepository EventZone { get; set; } =
        eventZoneRepository ?? throw new ArgumentNullException(nameof(eventZoneRepository));
}

