using Ticketing.Application.Model.DTOs;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.SysRole.Request;
using Ticketing.Infrastructure.DTOs.SysRole.Response;
using Ticketing.Infrastructure.Entities.SysRole.Response;

namespace Ticketing.Domain.Domain.SysRole.Interfaces;

public interface ISysRoleDomainService
{
    Task<ResponseMessage<int>> InsertAsync(SysRoleEntity entity, CancellationToken cancellationToken = default);
    Task<ResponseMessage<bool>> UpdateAsync(SysRoleEntity entity, CancellationToken cancellationToken = default);
    Task<ResponseMessage<bool>> DeleteAsync(SysRoleEntity entity, CancellationToken cancellationToken = default);
    Task<ResponseMessage<SysRoleDetailDto?>> GetByIdAsync(SysRoleGetByIdRequest request, CancellationToken cancellationToken = default);
    Task<ResponseMessage<IEnumerable<SysRoleListDto>>> GetPagedListAsync(SysRoleGetPagedListRequest request, CancellationToken cancellationToken = default);
}

