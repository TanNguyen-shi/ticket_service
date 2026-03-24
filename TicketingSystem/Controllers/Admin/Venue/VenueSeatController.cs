using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.UseCases.VenueSeat.Interfaces;
using Ticketing.Infrastructure.DTOs.VenueSeat.Request;
using Ticketing.Infrastructure.Helpers.Interfaces;

namespace TicketingSystem.Controllers.Admin.Venue;

[Route("api/admin/venue-seat")]
public class VenueSeatController(IVenueSeatUseCases venueSeatUseCases, IUserHelper user) : BaseApiController
{
    /// <summary>
    /// Admin - Khai báo ghế của địa điểm tổ chức sự kiện
    /// </summary>
    /// <param name="request">Input : Thông tin cấu hình ghế</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Id record vừa tạo</returns>
    [HttpPost("insert")]
    public async Task<IActionResult> Insert([FromBody] VenueSeatCreateRequest request, CancellationToken cancellationToken)
    {
        var result = await venueSeatUseCases.InsertAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Cập nhật thông tin ghế của địa điểm tổ chức sự kiện
    /// </summary>
    /// <param name="request">Input : Thông tin ghế cần cập nhật</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Kết quả cập nhật ghế</returns>
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] VenueSeatUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await venueSeatUseCases.UpdateAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Xóa mềm ghế của địa điểm tổ chức sự kiện
    /// </summary>
    /// <param name="request">Input : Id ghế cần xóa</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Kết quả xóa ghế</returns>
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromBody] VenueSeatDeleteRequest request, CancellationToken cancellationToken)
    {
        var result = await venueSeatUseCases.DeleteAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Lấy chi tiết một ghế theo Id
    /// </summary>
    /// <param name="request">Input : Id ghế cần lấy chi tiết</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Thông tin chi tiết ghế</returns>
    [HttpGet("getbyid")]
    public async Task<IActionResult> GetById([FromQuery] VenueSeatGetByIdRequest request, CancellationToken cancellationToken)
    {
        var result = await venueSeatUseCases.GetByIdAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Lấy danh sách ghế có phân trang
    /// </summary>
    /// <param name="request">Input : Điều kiện lọc + phân trang</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Danh sách ghế theo trang</returns>
    [HttpGet("getpagedlist")]
    public async Task<IActionResult> GetPagedList([FromQuery] VenueSeatGetPagedListRequest request, CancellationToken cancellationToken)
    {
        var result = await venueSeatUseCases.GetPagedListAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }
}
