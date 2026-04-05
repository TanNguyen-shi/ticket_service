using Ticketing.Application.Model.DTOs;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.SysUserRole.Request;
using Ticketing.Infrastructure.DTOs.SysUserRole.Response;

namespace Ticketing.Application.UseCases.SysUserRole.Interfaces;

public interface ISysUserRoleUseCases
{
    Task<ResponseMessage<int>?> InsertAsync(SysUserRoleCreateRequest request, long? userLogin, CancellationToken cancellationToken = default);
    Task<ResponseMessage<bool>> UpdateAsync(SysUserRoleUpdateRequest request, long? userLogin, CancellationToken cancellationToken = default);
    Task<ResponseMessage<bool>> DeleteAsync(SysUserRoleDeleteRequest request, long? userLogin, CancellationToken cancellationToken = default);
    Task<ResponseMessage<SysUserRoleDetailDto?>> GetByIdAsync(SysUserRoleGetByIdRequest request, long? userLogin, CancellationToken cancellationToken = default);
    Task<ResponseMessage<IEnumerable<SysUserRoleListDto>>> GetPagedListAsync(SysUserRoleGetPagedListRequest request, long? userLogin, CancellationToken cancellationToken = default);
}

