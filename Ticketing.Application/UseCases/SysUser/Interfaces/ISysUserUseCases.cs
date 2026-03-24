using Ticketing.Application.Model.DTOs;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.SysUser.Request;
using Ticketing.Infrastructure.DTOs.SysUser.Response;

namespace Ticketing.Application.UseCases.SysUser.Interfaces;

public interface ISysUserUseCases
{
    Task<ResponseMessage<int>?> InsertAsync(SysUserCreateRequest request, long? userLogin, CancellationToken cancellationToken = default);
    Task<ResponseMessage<bool>> UpdateAsync(SysUserUpdateRequest request, long? userLogin, CancellationToken cancellationToken = default);
    Task<ResponseMessage<bool>> DeleteAsync(SysUserDeleteRequest request, long? userLogin, CancellationToken cancellationToken = default);
    Task<ResponseMessage<SysUserDetailDto?>> GetByIdAsync(SysUserGetByIdRequest request, long? userLogin, CancellationToken cancellationToken = default);
    Task<ResponseMessage<IEnumerable<SysUserListDto>>> GetPagedListAsync(SysUserGetPagedListRequest request, long? userLogin, CancellationToken cancellationToken = default);
}

