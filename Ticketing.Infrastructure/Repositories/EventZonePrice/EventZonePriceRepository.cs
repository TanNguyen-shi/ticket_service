using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.EventZonePrice.Response;
using Ticketing.Infrastructure.Persistence.Helpers;

namespace Ticketing.Infrastructure.Repositories.EventZonePrice;

public interface IEventZonePriceRepository : IGenericRepository<EventZonePriceEntity>
{
}

public class EventZonePriceRepository(
    IDapperProcedureHelper dapper,
    DapperContextAccessor contextAccessor)
    : Repository<EventZonePriceEntity>(dapper, contextAccessor), IEventZonePriceRepository
{
    protected override string Schema => "ticketing";
    protected override string TableName => "event_zone_price";
}

