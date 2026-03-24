using Ticketing.Application.Model.DTOs;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.SysUser.Request;
using Ticketing.Infrastructure.DTOs.SysUser.Response;
using Ticketing.Infrastructure.Entities.SysUser.Response;

namespace Ticketing.Domain.Domain.SysUser.Interfaces;

public interface ISysUserDomainService
{
    Task<ResponseMessage<int>> InsertAsync(SysUserEntity entity, CancellationToken cancellationToken = default);
    Task<ResponseMessage<bool>> UpdateAsync(SysUserEntity entity, CancellationToken cancellationToken = default);
    Task<ResponseMessage<bool>> DeleteAsync(SysUserEntity entity, CancellationToken cancellationToken = default);
    Task<ResponseMessage<SysUserDetailDto?>> GetByIdAsync(SysUserGetByIdRequest request, CancellationToken cancellationToken = default);
    Task<ResponseMessage<IEnumerable<SysUserListDto>>> GetPagedListAsync(SysUserGetPagedListRequest request, CancellationToken cancellationToken = default);
}

