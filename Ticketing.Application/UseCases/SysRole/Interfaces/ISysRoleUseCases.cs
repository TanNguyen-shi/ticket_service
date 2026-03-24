using Ticketing.Application.Model.DTOs;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.SysRole.Request;
using Ticketing.Infrastructure.DTOs.SysRole.Response;

namespace Ticketing.Application.UseCases.SysRole.Interfaces;

public interface ISysRoleUseCases
{
    Task<ResponseMessage<int>?> InsertAsync(SysRoleCreateRequest request, long? userLogin, CancellationToken cancellationToken = default);
    Task<ResponseMessage<bool>> UpdateAsync(SysRoleUpdateRequest request, long? userLogin, CancellationToken cancellationToken = default);
    Task<ResponseMessage<bool>> DeleteAsync(SysRoleDeleteRequest request, long? userLogin, CancellationToken cancellationToken = default);
    Task<ResponseMessage<SysRoleDetailDto?>> GetByIdAsync(SysRoleGetByIdRequest request, long? userLogin, CancellationToken cancellationToken = default);
    Task<ResponseMessage<IEnumerable<SysRoleListDto>>> GetPagedListAsync(SysRoleGetPagedListRequest request, long? userLogin, CancellationToken cancellationToken = default);
}

