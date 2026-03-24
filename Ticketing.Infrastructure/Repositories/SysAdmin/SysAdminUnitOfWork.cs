using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Repositories.SysUserRole;

namespace Ticketing.Infrastructure.Repositories.SysAdmin;

public interface ISysAdminUnitOfWork : IUnitOfWork
{
    ISysRoleRepository SysRole { get; set; }
    ISysUserRepository SysUser { get; set; }
    ISysUserRoleRepository SysUserRole { get; set; }
}

public class SysAdminUnitOfWork(
    ISysRoleRepository sysRoleRepository,
    ISysUserRepository sysUserRepository,
    ISysUserRoleRepository sysUserRoleRepository,
    DapperContext dapperContext,
    DapperContextAccessor contextAccessor)
    : UnitOfWork(dapperContext, contextAccessor), ISysAdminUnitOfWork
{
    public ISysRoleRepository SysRole { get; set; } =
        sysRoleRepository ?? throw new ArgumentNullException(nameof(sysRoleRepository));

    public ISysUserRepository SysUser { get; set; } =
        sysUserRepository ?? throw new ArgumentNullException(nameof(sysUserRepository));

    public ISysUserRoleRepository SysUserRole { get; set; } =
        sysUserRoleRepository ?? throw new ArgumentNullException(nameof(sysUserRoleRepository));
}

