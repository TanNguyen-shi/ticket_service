using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.VenueSeat.Response;
using Ticketing.Infrastructure.Persistence.Helpers;

namespace Ticketing.Infrastructure.Repositories.VenueSeat;

public interface IVenueSeatRepository : IGenericRepository<VenueSeatEntity>
{
}

public class VenueSeatRepository(
    IDapperProcedureHelper dapper,
    DapperContextAccessor contextAccessor)
    : Repository<VenueSeatEntity>(dapper, contextAccessor), IVenueSeatRepository
{
    protected override string Schema => "ticketing";
    protected override string TableName => "venue_seat";
}

