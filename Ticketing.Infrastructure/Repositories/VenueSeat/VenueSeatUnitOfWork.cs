using Ticketing.Infrastructure.Configurations;

namespace Ticketing.Infrastructure.Repositories.VenueSeat;

public interface IVenueSeatUnitOfWork : IUnitOfWork
{
    IVenueSeatRepository VenueSeat { get; set; }
}

public class VenueSeatUnitOfWork(
    IVenueSeatRepository venueSeatRepository,
    DapperContext dapperContext,
    DapperContextAccessor contextAccessor)
    : UnitOfWork(dapperContext, contextAccessor), IVenueSeatUnitOfWork
{
    public IVenueSeatRepository VenueSeat { get; set; } =
        venueSeatRepository ?? throw new ArgumentNullException(nameof(venueSeatRepository));
}

