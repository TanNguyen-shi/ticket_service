using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.UseCases.SysRole.Interfaces;
using Ticketing.Infrastructure.DTOs.SysRole.Request;
using Ticketing.Infrastructure.Helpers.Interfaces;

namespace TicketingSystem.Controllers.Admin.SystemUser;

[Route("api/admin/event/sys-role")]
public class SysRoleController(ISysRoleUseCases sysRoleUseCases, IUserHelper user) : BaseApiController
{
    /// <summary>
    /// Admin - Khai bao vai tro he thong
    /// </summary>
    /// <param name="request">Input: Thong tin vai tro can tao moi</param>
    /// <param name="cancellationToken">Default -> dung de build Transaction</param>
    /// <returns>Id record vua tao</returns>
    [HttpPost("insert")]
    public async Task<IActionResult> Insert([FromBody] SysRoleCreateRequest request, CancellationToken cancellationToken)
    {
        var result = await sysRoleUseCases.InsertAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Cap nhat thong tin vai tro he thong
    /// </summary>
    /// <param name="request">Input: Thong tin vai tro can cap nhat</param>
    /// <param name="cancellationToken">Default -> dung de build Transaction</param>
    /// <returns>Ket qua cap nhat vai tro</returns>
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] SysRoleUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await sysRoleUseCases.UpdateAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Xoa mem vai tro he thong
    /// </summary>
    /// <param name="request">Input: Id vai tro can xoa</param>
    /// <param name="cancellationToken">Default -> dung de build Transaction</param>
    /// <returns>Ket qua xoa vai tro</returns>
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromBody] SysRoleDeleteRequest request, CancellationToken cancellationToken)
    {
        var result = await sysRoleUseCases.DeleteAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Lay chi tiet vai tro theo Id
    /// </summary>
    /// <param name="request">Input: Id vai tro can lay chi tiet</param>
    /// <param name="cancellationToken">Default -> dung de build Transaction</param>
    /// <returns>Thong tin chi tiet vai tro</returns>
    [HttpGet("getbyid")]
    public async Task<IActionResult> GetById([FromQuery] SysRoleGetByIdRequest request, CancellationToken cancellationToken)
    {
        var result = await sysRoleUseCases.GetByIdAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Lay danh sach vai tro co phan trang
    /// </summary>
    /// <param name="request">Input: Dieu kien loc + phan trang</param>
    /// <param name="cancellationToken">Default -> dung de build Transaction</param>
    /// <returns>Danh sach vai tro theo trang</returns>
    [HttpGet("getpagedlist")]
    public async Task<IActionResult> GetPagedList([FromQuery] SysRoleGetPagedListRequest request, CancellationToken cancellationToken)
    {
        var result = await sysRoleUseCases.GetPagedListAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }
}