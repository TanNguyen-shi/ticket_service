using Ticketing.Infrastructure.Configurations;

namespace Ticketing.Infrastructure.Repositories.Venue;

public interface IVenueUnitOfWork : IUnitOfWork
{
    IVenueRepository Venue { get; set; }
}

public class VenueUnitOfWork(
    IVenueRepository venueRepository,
    DapperContext dapperContext,
    DapperContextAccessor contextAccessor)
    : UnitOfWork(dapperContext, contextAccessor), IVenueUnitOfWork
{
    public IVenueRepository Venue { get; set; } = venueRepository ?? throw new ArgumentNullException(nameof(venueRepository));
}