using Ticketing.Application.Model.DTOs;
using Ticketing.Application.UseCases.SysUserRole.Interfaces;
using Ticketing.Domain.Domain.SysUserRole.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.SysUserRole.Request;
using Ticketing.Infrastructure.DTOs.SysUserRole.Response;
using Ticketing.Infrastructure.Entities.SysUserRole.Response;

namespace Ticketing.Application.UseCases.SysUserRole;

public class SysUserRoleUseCases(ISysUserRoleDomainService sysUserRoleDomain) : ISysUserRoleUseCases
{
    public async Task<ResponseMessage<int>?> InsertAsync(SysUserRoleCreateRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<int>();
        try
        {
            var insert = await sysUserRoleDomain.InsertAsync(new SysUserRoleEntity
            {
                user_id = request.user_id,
                role_id = request.role_id,
                assigned_at = request.assigned_at,
                assigned_by = userLogin
            }, cancellationToken);

            if (!insert.issuccess)
                return new ResponseMessage<int>().MessageError(insert.message ?? "Them moi phan quyen that bai");

            return new ResponseMessage<int>().MessageSuccess(insert.data, "Thanh cong");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> UpdateAsync(SysUserRoleUpdateRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<bool>();
        try
        {
            var update = await sysUserRoleDomain.UpdateAsync(new SysUserRoleEntity
            {
                user_role_id = request.user_role_id,
                user_id = request.user_id,
                role_id = request.role_id,
                assigned_at = request.assigned_at,
                assigned_by = userLogin
            }, cancellationToken);

            if (!update.issuccess)
                return new ResponseMessage<bool>().MessageError(update.message ?? "Cap nhat phan quyen that bai");

            return new ResponseMessage<bool>().MessageSuccess(true, "Thanh cong");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> DeleteAsync(SysUserRoleDeleteRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<bool>();
        try
        {
            var delete = await sysUserRoleDomain.DeleteAsync(new SysUserRoleEntity
            {
                user_role_id = request.user_role_id,
                assigned_by = userLogin
            }, cancellationToken);

            if (!delete.issuccess)
                return new ResponseMessage<bool>().MessageError(delete.message ?? "Xoa phan quyen that bai");

            return new ResponseMessage<bool>().MessageSuccess(true, "Thanh cong");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<SysUserRoleDetailDto?>> GetByIdAsync(SysUserRoleGetByIdRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        try
        {
            return await sysUserRoleDomain.GetByIdAsync(new SysUserRoleGetByIdRequest
            {
                user_role_id = request.user_role_id
            }, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<SysUserRoleDetailDto?>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<SysUserRoleListDto>>> GetPagedListAsync(SysUserRoleGetPagedListRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        try
        {
            return await sysUserRoleDomain.GetPagedListAsync(new SysUserRoleGetPagedListRequest
            {
                pagesize = request.pagesize,
                offset = request.offset,
                keysearch = request.keysearch,
                user_id = request.user_id,
                role_id = request.role_id
            }, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<SysUserRoleListDto>>().MessageError(e.Message);
        }
    }
}

