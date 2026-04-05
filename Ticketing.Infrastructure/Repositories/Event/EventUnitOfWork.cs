using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Repositories.EventSeatInventory;
using Ticketing.Infrastructure.Repositories.EventZone;
using Ticketing.Infrastructure.Repositories.EventZonePrice;
using Ticketing.Infrastructure.Repositories.EventZoneSection;

namespace Ticketing.Infrastructure.Repositories.Event;

public interface IEventUnitOfWork : IUnitOfWork
{
    IEventRepository Event { get; set; }
    IEventZoneRepository EventZone { get; set; }
    IEventZonePriceRepository EventZonePrice { get; set; }
    IEventZoneSectionRepository EventZoneSection { get; set; }
    IEventSeatInventoryRepository  EventSeatInventory { get; set; }
}

public class EventUnitOfWork(
    IEventRepository eventRepository,
    IEventZoneRepository eventZoneRepository,
    IEventZonePriceRepository eventZonePriceRepository,
    IEventZoneSectionRepository eventZoneSectionRepository,
    IEventSeatInventoryRepository eventSeatInventoryRepository,
    DapperContext dapperContext,
    DapperContextAccessor contextAccessor)
    : UnitOfWork(dapperContext, contextAccessor), IEventUnitOfWork
{
    public IEventRepository Event { get; set; } = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));

    public IEventZoneRepository EventZone { get; set; } =
        eventZoneRepository ?? throw new ArgumentNullException(nameof(eventZoneRepository));

    public IEventZonePriceRepository EventZonePrice { get; set; } =
        eventZonePriceRepository ?? throw new ArgumentNullException(nameof(eventZonePriceRepository));

    public IEventZoneSectionRepository EventZoneSection { get; set; } =
        eventZoneSectionRepository ?? throw new ArgumentNullException(nameof(eventZoneSectionRepository));
    public IEventSeatInventoryRepository EventSeatInventory { get; set; } = eventSeatInventoryRepository ?? throw new ArgumentNullException(nameof(eventSeatInventoryRepository));
}