using Ticketing.Application.Model.DTOs;
using Ticketing.Application.UseCases.SysUser.Interfaces;
using Ticketing.Domain.Domain.SysUser.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.SysUser.Request;
using Ticketing.Infrastructure.DTOs.SysUser.Response;
using Ticketing.Infrastructure.Entities.SysUser.Response;
using Ticketing.Infrastructure.Helpers.Interfaces;

namespace Ticketing.Application.UseCases.SysUser;

public class SysUserUseCases(
    ISysUserDomainService sysUserDomain,
    IPasswordHelper passwordHelper) : ISysUserUseCases
{
    public async Task<ResponseMessage<int>?> InsertAsync(SysUserCreateRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<int>();
        try
        {
            var insert = await sysUserDomain.InsertAsync(new SysUserEntity
            {
                username = request.username,
                email = request.email,
                phone = request.phone,
                password_hash = passwordHelper.HashPassword(request.password_hash),
                full_name = request.full_name,
                user_type = request.user_type,
                status = request.status,
                last_login_at = request.last_login_at,
                created_by = userLogin
            }, cancellationToken);

            if (!insert.issuccess)
                return new ResponseMessage<int>().MessageError(insert.message ?? "Them moi nguoi dung that bai");

            return new ResponseMessage<int>().MessageSuccess(insert.data, "Thanh cong");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> UpdateAsync(SysUserUpdateRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<bool>();
        try
        {
            var update = await sysUserDomain.UpdateAsync(new SysUserEntity
            {
                user_id = request.user_id,
                username = request.username,
                email = request.email,
                phone = request.phone,
                password_hash = passwordHelper.HashPassword(request.password_hash),
                full_name = request.full_name,
                user_type = request.user_type,
                status = request.status,
                last_login_at = request.last_login_at,
                updated_by = userLogin
            }, cancellationToken);

            if (!update.issuccess)
                return new ResponseMessage<bool>().MessageError(update.message ?? "Cap nhat nguoi dung that bai");

            return new ResponseMessage<bool>().MessageSuccess(true, "Thanh cong");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> DeleteAsync(SysUserDeleteRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<bool>();
        try
        {
            var delete = await sysUserDomain.DeleteAsync(new SysUserEntity
            {
                user_id = request.user_id,
                updated_by = userLogin
            }, cancellationToken);

            if (!delete.issuccess)
                return new ResponseMessage<bool>().MessageError(delete.message ?? "Xoa nguoi dung that bai");

            return new ResponseMessage<bool>().MessageSuccess(true, "Thanh cong");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<SysUserDetailDto?>> GetByIdAsync(SysUserGetByIdRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        try
        {
            return await sysUserDomain.GetByIdAsync(new SysUserGetByIdRequest
            {
                user_id = request.user_id
            }, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<SysUserDetailDto?>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<SysUserListDto>>> GetPagedListAsync(SysUserGetPagedListRequest request, long? userLogin, CancellationToken cancellationToken = default)
    {
        try
        {
            return await sysUserDomain.GetPagedListAsync(new SysUserGetPagedListRequest
            {
                pagesize = request.pagesize,
                offset = request.offset,
                keysearch = request.keysearch,
                user_type = request.user_type,
                status = request.status
            }, cancellationToken);
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<SysUserListDto>>().MessageError(e.Message);
        }
    }
}

