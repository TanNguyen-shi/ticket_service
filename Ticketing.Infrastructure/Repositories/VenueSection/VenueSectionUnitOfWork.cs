using Ticketing.Infrastructure.Configurations;

namespace Ticketing.Infrastructure.Repositories.VenueSection;

public interface IVenueSectionUnitOfWork : IUnitOfWork
{
    IVenueSectionRepository VenueSection { get; set; }
}

public class VenueSectionUnitOfWork(
    IVenueSectionRepository venueSectionRepository,
    DapperContext dapperContext,
    DapperContextAccessor contextAccessor)
    : UnitOfWork(dapperContext, contextAccessor), IVenueSectionUnitOfWork
{
    public IVenueSectionRepository VenueSection { get; set; } =
        venueSectionRepository ?? throw new ArgumentNullException(nameof(venueSectionRepository));
}

