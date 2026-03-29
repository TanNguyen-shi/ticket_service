using Ticketing.Infrastructure.Configurations;

namespace Ticketing.Infrastructure.Repositories.EventZonePrice;

public interface IEventZonePriceUnitOfWork : IUnitOfWork
{
    IEventZonePriceRepository EventZonePrice { get; set; }
}

public class EventZonePriceUnitOfWork(
    IEventZonePriceRepository eventZonePriceRepository,
    DapperContext dapperContext,
    DapperContextAccessor contextAccessor)
    : UnitOfWork(dapperContext, contextAccessor), IEventZonePriceUnitOfWork
{
    public IEventZonePriceRepository EventZonePrice { get; set; } =
        eventZonePriceRepository ?? throw new ArgumentNullException(nameof(eventZonePriceRepository));
}

