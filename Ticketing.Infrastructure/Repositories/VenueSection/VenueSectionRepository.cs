using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.VenueSection.Response;
using Ticketing.Infrastructure.Persistence.Helpers;

namespace Ticketing.Infrastructure.Repositories.VenueSection;

public interface IVenueSectionRepository : IGenericRepository<VenueSectionEntity>
{
}

public class VenueSectionRepository(
    IDapperProcedureHelper dapper,
    DapperContextAccessor contextAccessor)
    : Repository<VenueSectionEntity>(dapper, contextAccessor), IVenueSectionRepository
{
    protected override string Schema => "ticketing";
    protected override string TableName => "venue_section";
}

