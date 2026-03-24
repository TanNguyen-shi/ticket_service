using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.SysRole.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Persistence.Helpers;

namespace Ticketing.Infrastructure.Repositories.SysAdmin;

public interface ISysRoleRepository : IGenericRepository<SysRoleEntity>
{
}

public class SysRoleRepository(
    IDapperProcedureHelper dapper,
    DapperContextAccessor contextAccessor)
    : Repository<SysRoleEntity>(dapper, contextAccessor), ISysRoleRepository
{
    protected override string Schema => "ticketing";
    protected override string TableName => "sys_role";
}