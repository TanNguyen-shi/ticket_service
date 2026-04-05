using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.VenueSeat.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Persistence.Helpers;

namespace Ticketing.Infrastructure.Repositories.VenueSeat;

public interface IVenueSeatRepository : IGenericRepository<VenueSeatEntity>
{
    Task<IEnumerable<TResult>> GetBySectionId<TResult>(object param, CancellationToken cancellationToken = default);
    Task<IEnumerable<TResult>> GetBySectionIds<TResult>(object param, CancellationToken cancellationToken = default);
}

public class VenueSeatRepository(
    IDapperProcedureHelper dapper,
    DapperContextAccessor contextAccessor)
    : Repository<VenueSeatEntity>(dapper, contextAccessor), IVenueSeatRepository
{
    protected override string Schema => "ticketing";
    protected override string TableName => "venue_seat";

    public async Task<IEnumerable<TResult>> GetBySectionId<TResult>(object param, CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("getsectionid");
        return await _dapper.GetAllAsync<TResult>(
            Connection,
            spName,
            param?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }

    public async Task<IEnumerable<TResult>> GetBySectionIds<TResult>(object param, CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("getbysectionids");
        return await _dapper.GetAllAsync<TResult>(
            Connection,
            spName,
            param?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }
}