using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.SysUserRole.Response;
using Ticketing.Infrastructure.Persistence.Helpers;

namespace Ticketing.Infrastructure.Repositories.SysUserRole;

public interface ISysUserRoleRepository : IGenericRepository<SysUserRoleEntity>
{
}

public class SysUserRoleRepository(
    IDapperProcedureHelper dapper,
    DapperContextAccessor contextAccessor)
    : Repository<SysUserRoleEntity>(dapper, contextAccessor), ISysUserRoleRepository
{
    protected override string Schema => "ticketing";
    protected override string TableName => "sys_user_role";
}

