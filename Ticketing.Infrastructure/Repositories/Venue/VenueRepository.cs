using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.Venue;
using Ticketing.Infrastructure.Entities.Venue.Response;
using Ticketing.Infrastructure.Persistence.Helpers;

namespace Ticketing.Infrastructure.Repositories.Venue;

public interface IVenueRepository : IGenericRepository<VenueEntity>
{
    Task<IEnumerable<VenueEntity>> GetByStatusAsync(int status, CancellationToken cancellationToken = default);
}

public class VenueRepository(IDapperProcedureHelper dapper, DapperContextAccessor contextAccessor) : Repository<VenueEntity>(dapper, contextAccessor), IVenueRepository
{
    protected override string Schema => "ticketing";
    protected override string TableName => "venue";

    public async Task<IEnumerable<VenueEntity>> GetByStatusAsync(int status, CancellationToken cancellationToken = default)
    {
        var spName = $"{Schema}.sp_event_get_by_status";
        var parameters = new object[]
        {
            "status", status
        };
        return await _dapper.GetAllAsync<VenueEntity>(Connection, spName, parameters, 30, Transaction, cancellationToken);
    }
}