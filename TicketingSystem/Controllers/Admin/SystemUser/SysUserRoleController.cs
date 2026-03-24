using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.UseCases.SysUserRole.Interfaces;
using Ticketing.Infrastructure.DTOs.SysUserRole.Request;
using Ticketing.Infrastructure.Helpers.Interfaces;

namespace TicketingSystem.Controllers.Admin.Event;

[Route("api/admin/event/sys-user-role")]
public class SysUserRoleController(ISysUserRoleUseCases sysUserRoleUseCases, IUserHelper user) : BaseApiController
{
    /// <summary>
    /// Admin - Khai bao phan quyen nguoi dung
    /// </summary>
    /// <param name="request">Input: Thong tin phan quyen can tao moi</param>
    /// <param name="cancellationToken">Default -> dung de build Transaction</param>
    /// <returns>Id record vua tao</returns>
    [HttpPost("insert")]
    public async Task<IActionResult> Insert([FromBody] SysUserRoleCreateRequest request, CancellationToken cancellationToken)
    {
        var result = await sysUserRoleUseCases.InsertAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Cap nhat thong tin phan quyen nguoi dung
    /// </summary>
    /// <param name="request">Input: Thong tin phan quyen can cap nhat</param>
    /// <param name="cancellationToken">Default -> dung de build Transaction</param>
    /// <returns>Ket qua cap nhat phan quyen</returns>
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] SysUserRoleUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await sysUserRoleUseCases.UpdateAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Xoa mem phan quyen nguoi dung
    /// </summary>
    /// <param name="request">Input: Id phan quyen can xoa</param>
    /// <param name="cancellationToken">Default -> dung de build Transaction</param>
    /// <returns>Ket qua xoa phan quyen</returns>
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromBody] SysUserRoleDeleteRequest request, CancellationToken cancellationToken)
    {
        var result = await sysUserRoleUseCases.DeleteAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Lay chi tiet phan quyen theo Id
    /// </summary>
    /// <param name="request">Input: Id phan quyen can lay chi tiet</param>
    /// <param name="cancellationToken">Default -> dung de build Transaction</param>
    /// <returns>Thong tin chi tiet phan quyen</returns>
    [HttpGet("getbyid")]
    public async Task<IActionResult> GetById([FromQuery] SysUserRoleGetByIdRequest request, CancellationToken cancellationToken)
    {
        var result = await sysUserRoleUseCases.GetByIdAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Lay danh sach phan quyen co phan trang
    /// </summary>
    /// <param name="request">Input: Dieu kien loc + phan trang</param>
    /// <param name="cancellationToken">Default -> dung de build Transaction</param>
    /// <returns>Danh sach phan quyen theo trang</returns>
    [HttpGet("getpagedlist")]
    public async Task<IActionResult> GetPagedList([FromQuery] SysUserRoleGetPagedListRequest request, CancellationToken cancellationToken)
    {
        var result = await sysUserRoleUseCases.GetPagedListAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }
}

