using Ticketing.Application.Model.DTOs;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.SysUserRole.Request;
using Ticketing.Infrastructure.DTOs.SysUserRole.Response;
using Ticketing.Infrastructure.Entities.SysUserRole.Response;

namespace Ticketing.Domain.Domain.SysUserRole.Interfaces;

public interface ISysUserRoleDomainService
{
    Task<ResponseMessage<int>> InsertAsync(SysUserRoleEntity entity, CancellationToken cancellationToken = default);
    Task<ResponseMessage<bool>> UpdateAsync(SysUserRoleEntity entity, CancellationToken cancellationToken = default);
    Task<ResponseMessage<bool>> DeleteAsync(SysUserRoleEntity entity, CancellationToken cancellationToken = default);
    Task<ResponseMessage<SysUserRoleDetailDto?>> GetByIdAsync(SysUserRoleGetByIdRequest request, CancellationToken cancellationToken = default);
    Task<ResponseMessage<IEnumerable<SysUserRoleListDto>>> GetPagedListAsync(SysUserRoleGetPagedListRequest request, CancellationToken cancellationToken = default);
}

