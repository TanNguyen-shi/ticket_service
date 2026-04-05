using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Repositories.VenueSeat;
using Ticketing.Infrastructure.Repositories.VenueSection;

namespace Ticketing.Infrastructure.Repositories.Venue;

public interface IVenueUnitOfWork : IUnitOfWork
{
    IVenueRepository Venue { get; set; }
    IVenueSeatRepository VenueSeat { get; set; }
    IVenueSectionRepository VenueSection { get; set; }
}

public class VenueUnitOfWork(
    IVenueRepository venueRepository,
    IVenueSeatRepository seatRepository,
    IVenueSectionRepository venueSectionRepository,
    DapperContext dapperContext,
    DapperContextAccessor contextAccessor)
    : UnitOfWork(dapperContext, contextAccessor), IVenueUnitOfWork
{
    public IVenueRepository Venue { get; set; } = venueRepository ?? throw new ArgumentNullException(nameof(venueRepository));
    public IVenueSeatRepository VenueSeat { get; set; } = seatRepository ?? throw new ArgumentNullException(nameof(seatRepository));
    public IVenueSectionRepository VenueSection { get; set; } = venueSectionRepository ?? throw new ArgumentNullException(nameof(venueSectionRepository));
}