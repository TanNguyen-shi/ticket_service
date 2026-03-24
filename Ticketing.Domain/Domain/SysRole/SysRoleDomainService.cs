using Ticketing.Application.Model.DTOs;
using Ticketing.Domain.Domain.SysRole.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.SysRole.Request;
using Ticketing.Infrastructure.DTOs.SysRole.Response;
using Ticketing.Infrastructure.Entities;
using Ticketing.Infrastructure.Entities.SysRole.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Repositories.SysAdmin;

namespace Ticketing.Domain.Domain.SysRole;

public class SysRoleDomainService(ISysAdminUnitOfWork unitOfWork)
    : BaseService<ISysRoleRepository, SysRoleEntity>(unitOfWork.SysRole, TicketingTypeEnum.SysRole), ISysRoleDomainService
{
    public async Task<ResponseMessage<int>> InsertAsync(SysRoleEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);
        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.InsertAsync(new
            {
                role_code = entity.role_code,
                role_name = entity.role_name,
                description = entity.description,
                status = entity.status,
                created_by = entity.created_by
            }, cancellationToken)!.ToIntAsync();

            if (result is not > 0)
                throw new Exception("Them moi vai tro that bai");

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<int>().MessageSuccess(result ?? 0, "Thanh cong");
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<int>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> UpdateAsync(SysRoleEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);
        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.UpdateAsync(new
            {
                role_id = entity.role_id,
                role_code = entity.role_code,
                role_name = entity.role_name,
                description = entity.description,
                status = entity.status,
                updated_by = entity.updated_by
            }, cancellationToken)!.ToBoolAsync();

            if (!result)
                throw new Exception("Cap nhat vai tro that bai");

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageSuccess(true, "Thanh cong");
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<bool>> DeleteAsync(SysRoleEntity entity, CancellationToken cancellationToken = default)
    {
        await unitOfWork.OpenAsync(cancellationToken);
        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await _repository.DeleteAsync(new
            {
                role_id = entity.role_id,
                updated_by = entity.updated_by
            }, cancellationToken)!.ToBoolAsync();

            if (!result)
                throw new Exception("Xoa vai tro that bai");

            await unitOfWork.CommitAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageSuccess(true, "Thanh cong");
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackAsync(cancellationToken: cancellationToken);
            return new ResponseMessage<bool>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<SysRoleDetailDto?>> GetByIdAsync(SysRoleGetByIdRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetAsync<SysRoleDetailDto>(new
            {
                role_id = request.role_id
            }, cancellationToken);

            if (result is null)
                return new ResponseMessage<SysRoleDetailDto?>().MessageWarning("Khong tim thay du lieu");

            return new ResponseMessage<SysRoleDetailDto?>().MessageSuccess(result, "Thanh cong");
        }
        catch (Exception e)
        {
            return new ResponseMessage<SysRoleDetailDto?>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<IEnumerable<SysRoleListDto>>> GetPagedListAsync(SysRoleGetPagedListRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _repository.GetPagedAsync<SysRoleListDto>(new
            {
                pagesize = request.pagesize,
                offset = request.offset,
                keysearch = request.keysearch,
                status = request.status
            }, cancellationToken);

            return new ResponseMessage<IEnumerable<SysRoleListDto>>().MessageSuccess(result ?? [], "Thanh cong");
        }
        catch (Exception e)
        {
            return new ResponseMessage<IEnumerable<SysRoleListDto>>().MessageError(e.Message);
        }
    }
}

