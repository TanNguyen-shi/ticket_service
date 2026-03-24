using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.SysUser.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Persistence.Helpers;

namespace Ticketing.Infrastructure.Repositories.SysAdmin;

public interface ISysUserRepository : IGenericRepository<SysUserEntity>
{
    Task<TResult?> GetByUserAsync<TResult, TParam>(TParam param, CancellationToken cancellationToken = default);
}

public class SysUserRepository(
    IDapperProcedureHelper dapper,
    DapperContextAccessor contextAccessor)
    : Repository<SysUserEntity>(dapper, contextAccessor), ISysUserRepository
{
    protected override string Schema => "ticketing";
    protected override string TableName => "sys_user";


    public async Task<TResult?> GetByUserAsync<TResult, TParam>(
        TParam param,
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("getbyuser");

        return await _dapper.GetAsync<TResult>(
            Connection,
            spName,
            param?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }
}