using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.EventZonePrice.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Persistence.Helpers;

namespace Ticketing.Infrastructure.Repositories.EventZonePrice;

public interface IEventZonePriceRepository : IGenericRepository<EventZonePriceEntity>
{
    Task<IEnumerable<TResult>> GetByZoneId<TResult>(object param, CancellationToken cancellationToken = default);
}

public class EventZonePriceRepository(
    IDapperProcedureHelper dapper,
    DapperContextAccessor contextAccessor)
    : Repository<EventZonePriceEntity>(dapper, contextAccessor), IEventZonePriceRepository
{
    protected override string Schema => "ticketing";
    protected override string TableName => "event_zone_price";

    public async Task<IEnumerable<TResult>> GetByZoneId<TResult>(object param, CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("getbyzoneid");
        return await _dapper.GetAllAsync<TResult>(
            Connection,
            spName,
            param?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }
}