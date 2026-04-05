using Ticketing.Application.Model.DTOs;
using Ticketing.Application.UseCases.SysRole.Interfaces;
using Ticketing.Domain.Domain.SysRole.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.SysRole.Request;
using Ticketing.Infrastructure.DTOs.SysRole.Response;
using Ticketing.Infrastructure.Entities.SysRole.Response;

namespace Ticketing.Application.UseCases.SysRole;

public class SysRoleUseCases(ISysRoleDomainService sysRoleDomain) : ISysRoleUseCases
{
    public async Task<ResponseMessage<int>?> InsertAsync(SysRoleCreateRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<int>();
        try
        {
            var insert = await sysRoleDomain.InsertAsync(new SysRoleEntity
            {
                role_code = request.role_code,
                role_name = request.role_name,
                description = request.description,
                status = request.status,
                created_by = userLogin
            }, cancellationToken);

            if (!insert.issuccess)
                return new ResponseMessage<int>().MessageError(insert.message ?? "Them moi vai tro that bai");

            return new ResponseMessage<int>().MessageSuccess(insert.data, "Thanh cong");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> UpdateAsync(SysRoleUpdateRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<bool>();
        try
        {
            var update = await sysRoleDomain.UpdateAsync(new SysRoleEntity
            {
                role_id = request.role_id,
                role_code = request.role_code,
                role_name = request.role_name,
                description = request.description,
                status = request.status,
                updated_by = userLogin
            }, cancellationToken);

            if (!update.issuccess)
                return new ResponseMessage<bool>().MessageError(update.message ?? "Cap nhat vai tro that bai");

            return new ResponseMessage<bool>().MessageSuccess(true, "Thanh cong");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> DeleteAsync(SysRoleDeleteRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<bool>();
        try
        {
            var delete = await sysRoleDomain.DeleteAsync(new SysRoleEntity
            {
                role_id = request.role_id,
                updated_by = userLogin
            }, cancellationToken);

            if (!delete.issuccess)
                return new ResponseMessage<bool>().MessageError(delete.message ?? "Xoa vai tro that bai");

            return new ResponseMessage<bool>().MessageSuccess(true, "Thanh cong");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<SysRoleDetailDto?>> GetByIdAsync(SysRoleGetByIdRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        try
        {
            return await sysRoleDomain.GetByIdAsync(new SysRoleGetByIdRequest
            {
                role_id = request.role_id
            }, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<SysRoleDetailDto?>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<SysRoleListDto>>> GetPagedListAsync(SysRoleGetPagedListRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        try
        {
            return await sysRoleDomain.GetPagedListAsync(new SysRoleGetPagedListRequest
            {
                pagesize = request.pagesize,
                offset = request.offset,
                keysearch = request.keysearch,
                status = request.status
            }, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<SysRoleListDto>>().MessageError(e.Message);
        }
    }
}

