using Ticketing.Application.Model.DTOs;
using Ticketing.Domain.Domain.SysUser.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.SysUser.Request;
using Ticketing.Infrastructure.DTOs.SysUser.Response;
using Ticketing.Infrastructure.Entities;
using Ticketing.Infrastructure.Entities.SysUser.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Repositories.SysAdmin;

namespace Ticketing.Domain.Domain.SysUser;

public class SysUserDomainService(ISysAdminUnitOfWork unitOfWork)
    : BaseService<ISysUserRepository, SysUserEntity>(unitOfWork.SysUser, TicketingTypeEnum.SysUser), ISysUserDomainService
{
    public async Task<ResponseMessage<int>> InsertAsync(SysUserEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);
        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.InsertAsync(new
            {
                username = entity.username,
                email = entity.email,
                phone = entity.phone,
                password_hash = entity.password_hash,
                full_name = entity.full_name,
                user_type = entity.user_type,
                status = entity.status,
                last_login_at = entity.last_login_at,
                created_by = entity.created_by
            }, cancellationToken)!.ToIntAsync();

            if (result is not > 0)
                throw new Exception("Them moi nguoi dung that bai");

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<int>().MessageSuccess(result ?? 0, "Thanh cong");
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<int>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> UpdateAsync(SysUserEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);
        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.UpdateAsync(new
            {
                user_id = entity.user_id,
                username = entity.username,
                email = entity.email,
                phone = entity.phone,
                password_hash = entity.password_hash,
                full_name = entity.full_name,
                user_type = entity.user_type,
                status = entity.status,
                last_login_at = entity.last_login_at,
                updated_by = entity.updated_by
            }, cancellationToken)!.ToBoolAsync();

            if (!result)
                throw new Exception("Cap nhat nguoi dung that bai");

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageSuccess(true, "Thanh cong");
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> DeleteAsync(SysUserEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);
        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.DeleteAsync(new
            {
                user_id = entity.user_id,
                updated_by = entity.updated_by
            }, cancellationToken)!.ToBoolAsync();

            if (!result)
                throw new Exception("Xoa nguoi dung that bai");

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageSuccess(true, "Thanh cong");
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<SysUserDetailDto?>> GetByIdAsync(SysUserGetByIdRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetAsync<SysUserDetailDto>(new
            {
                user_id = request.user_id
            }, cancellationToken);

            if (result is null)
                return new ResponseMessage<SysUserDetailDto?>().MessageWarning("Khong tim thay du lieu");

            return new ResponseMessage<SysUserDetailDto?>().MessageSuccess(result, "Thanh cong");
        }
        catch (Exception e)
        {
            return new ResponseMessage<SysUserDetailDto?>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<SysUserListDto>>> GetPagedListAsync(SysUserGetPagedListRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetPagedAsync<SysUserListDto>(new
            {
                pagesize = request.pagesize,
                offset = request.offset,
                keysearch = request.keysearch,
                user_type = request.user_type,
                status = request.status
            }, cancellationToken);

            return new ResponseMessage<IEnumerable<SysUserListDto>>().MessageSuccess(result ?? [], "Thanh cong");
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<SysUserListDto>>().MessageError(e.Message);
        }
    }
}

