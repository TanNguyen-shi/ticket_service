using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.UseCases.SysUser.Interfaces;
using Ticketing.Infrastructure.DTOs.SysUser.Request;
using Ticketing.Infrastructure.Helpers.Interfaces;

namespace TicketingSystem.Controllers.Admin.Event;

[Route("api/admin/event/sys-user")]
public class SysUserController(ISysUserUseCases sysUserUseCases, IUserHelper user) : BaseApiController
{
    /// <summary>
    /// Admin - Khai bao nguoi dung he thong
    /// </summary>
    /// <param name="request">Input: Thong tin nguoi dung can tao moi</param>
    /// <param name="cancellationToken">Default -> dung de build Transaction</param>
    /// <returns>Id record vua tao</returns>
    [HttpPost("insert")]
    public async Task<IActionResult> Insert([FromBody] SysUserCreateRequest request, CancellationToken cancellationToken)
    {
        var result = await sysUserUseCases.InsertAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Cap nhat thong tin nguoi dung he thong
    /// </summary>
    /// <param name="request">Input: Thong tin nguoi dung can cap nhat</param>
    /// <param name="cancellationToken">Default -> dung de build Transaction</param>
    /// <returns>Ket qua cap nhat nguoi dung</returns>
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] SysUserUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await sysUserUseCases.UpdateAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Xoa mem nguoi dung he thong
    /// </summary>
    /// <param name="request">Input: Id nguoi dung can xoa</param>
    /// <param name="cancellationToken">Default -> dung de build Transaction</param>
    /// <returns>Ket qua xoa nguoi dung</returns>
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromBody] SysUserDeleteRequest request, CancellationToken cancellationToken)
    {
        var result = await sysUserUseCases.DeleteAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Lay chi tiet nguoi dung theo Id
    /// </summary>
    /// <param name="request">Input: Id nguoi dung can lay chi tiet</param>
    /// <param name="cancellationToken">Default -> dung de build Transaction</param>
    /// <returns>Thong tin chi tiet nguoi dung</returns>
    [HttpGet("getbyid")]
    public async Task<IActionResult> GetById([FromQuery] SysUserGetByIdRequest request, CancellationToken cancellationToken)
    {
        var result = await sysUserUseCases.GetByIdAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Lay danh sach nguoi dung co phan trang
    /// </summary>
    /// <param name="request">Input: Dieu kien loc + phan trang</param>
    /// <param name="cancellationToken">Default -> dung de build Transaction</param>
    /// <returns>Danh sach nguoi dung theo trang</returns>
    [HttpGet("getpagedlist")]
    public async Task<IActionResult> GetPagedList([FromQuery] SysUserGetPagedListRequest request, CancellationToken cancellationToken)
    {
        var result = await sysUserUseCases.GetPagedListAsync(request, user.UserId, cancellationToken);
        return Ok(result);
    }
}

