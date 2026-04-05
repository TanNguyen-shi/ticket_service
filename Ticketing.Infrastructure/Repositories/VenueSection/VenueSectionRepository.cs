using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.VenueSection.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Persistence.Helpers;

namespace Ticketing.Infrastructure.Repositories.VenueSection;

public interface IVenueSectionRepository : IGenericRepository<VenueSectionEntity>
{
    Task<IEnumerable<TResult>> GetByVenueIdAsync<TResult>(object param, CancellationToken cancellationToken = default);
}

public class VenueSectionRepository(
    IDapperProcedureHelper dapper,
    DapperContextAccessor contextAccessor)
    : Repository<VenueSectionEntity>(dapper, contextAccessor), IVenueSectionRepository
{
    protected override string Schema => "ticketing";
    protected override string TableName => "venue_section";

    public async Task<IEnumerable<TResult>> GetByVenueIdAsync<TResult>(
        object param,
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("getbyvenueid");

        return await _dapper.GetAllAsync<TResult>(
            Connection,
            spName,
            param?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }
}