using Ticketing.Application.Model.DTOs;
using Ticketing.Domain.Domain.SysUserRole.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.SysUserRole.Request;
using Ticketing.Infrastructure.DTOs.SysUserRole.Response;
using Ticketing.Infrastructure.Entities;
using Ticketing.Infrastructure.Entities.SysUserRole.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Repositories.SysAdmin;
using Ticketing.Infrastructure.Repositories.SysUserRole;

namespace Ticketing.Domain.Domain.SysUserRole;

public class SysUserRoleDomainService(ISysAdminUnitOfWork unitOfWork)
    : BaseService<ISysUserRoleRepository, SysUserRoleEntity>(unitOfWork.SysUserRole, TicketingTypeEnum.SysUserRole), ISysUserRoleDomainService
{
    public async Task<ResponseMessage<int>> InsertAsync(SysUserRoleEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);
        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.InsertAsync(new
            {
                user_id = entity.user_id,
                role_id = entity.role_id,
                assigned_at = entity.assigned_at,
                assigned_by = entity.assigned_by
            }, cancellationToken)!.ToIntAsync();

            if (result is not > 0)
                throw new Exception("Them moi phan quyen that bai");

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<int>().MessageSuccess(result ?? 0, "Thanh cong");
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<int>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> UpdateAsync(SysUserRoleEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);
        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.UpdateAsync(new
            {
                user_role_id = entity.user_role_id,
                user_id = entity.user_id,
                role_id = entity.role_id,
                assigned_at = entity.assigned_at,
                assigned_by = entity.assigned_by
            }, cancellationToken)!.ToBoolAsync();

            if (!result)
                throw new Exception("Cap nhat phan quyen that bai");

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageSuccess(true, "Thanh cong");
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> DeleteAsync(SysUserRoleEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);
        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.DeleteAsync(new
            {
                user_role_id = entity.user_role_id,
                assigned_by = entity.assigned_by
            }, cancellationToken)!.ToBoolAsync();

            if (!result)
                throw new Exception("Xoa phan quyen that bai");

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageSuccess(true, "Thanh cong");
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<SysUserRoleDetailDto?>> GetByIdAsync(SysUserRoleGetByIdRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetAsync<SysUserRoleDetailDto>(new
            {
                user_role_id = request.user_role_id
            }, cancellationToken);

            if (result is null)
                return new ResponseMessage<SysUserRoleDetailDto?>().MessageWarning("Khong tim thay du lieu");

            return new ResponseMessage<SysUserRoleDetailDto?>().MessageSuccess(result, "Thanh cong");
        }
        catch (Exception e)
        {
            return new ResponseMessage<SysUserRoleDetailDto?>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<SysUserRoleListDto>>> GetPagedListAsync(SysUserRoleGetPagedListRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetPagedAsync<SysUserRoleListDto>(new
            {
                pagesize = request.pagesize,
                offset = request.offset,
                keysearch = request.keysearch,
                user_id = request.user_id,
                role_id = request.role_id
            }, cancellationToken);

            return new ResponseMessage<IEnumerable<SysUserRoleListDto>>().MessageSuccess(result ?? [], "Thanh cong");
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<SysUserRoleListDto>>().MessageError(e.Message);
        }
    }
}

