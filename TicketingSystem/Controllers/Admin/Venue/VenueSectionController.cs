using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.UseCases.VenueSection.Interfaces;
using Ticketing.Infrastructure.DTOs.VenueSection.Request;
using Ticketing.Infrastructure.Helpers.Interfaces;

namespace TicketingSystem.Controllers.Admin.Venue;

[Route("api/admin/venue-section")]
public class VenueSectionController(IVenueSectionUseCases venueSectionUseCases, IUserHelper user) : BaseApiController
{
    /// <summary>
    /// Admin - Khai báo khu vực của địa điểm tổ chức sự kiện
    /// </summary>
    /// <param name="request">Input : Thông tin cấu hình khu vực</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Id record vừa tạo</returns>
    [HttpPost("insert")]
    public async Task<IActionResult> Insert([FromBody] VenueSectionCreateRequest request, CancellationToken cancellationToken)
    {
        var result = await venueSectionUseCases.InsertAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Cập nhật thông tin khu vực của địa điểm tổ chức sự kiện
    /// </summary>
    /// <param name="request">Input : Thông tin khu vực cần cập nhật</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Kết quả cập nhật khu vực</returns>
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] VenueSectionUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await venueSectionUseCases.UpdateAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Xóa mềm khu vực của địa điểm tổ chức sự kiện
    /// </summary>
    /// <param name="request">Input : Id khu vực cần xóa</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Kết quả xóa khu vực</returns>
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromBody] VenueSectionDeleteRequest request, CancellationToken cancellationToken)
    {
        var result = await venueSectionUseCases.DeleteAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Lấy chi tiết một khu vực theo Id
    /// </summary>
    /// <param name="request">Input : Id khu vực cần lấy chi tiết</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Thông tin chi tiết khu vực</returns>
    [HttpGet("getbyid")]
    public async Task<IActionResult> GetById([FromQuery] VenueSectionGetByIdRequest request, CancellationToken cancellationToken)
    {
        var result = await venueSectionUseCases.GetByIdAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Lấy danh sách khu vực có phân trang
    /// </summary>
    /// <param name="request">Input : Điều kiện lọc + phân trang</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Danh sách khu vực theo trang</returns>
    [HttpGet("getpagedlist")]
    public async Task<IActionResult> GetPagedList([FromQuery] VenueSectionGetPagedListRequest request, CancellationToken cancellationToken)
    {
        var result = await venueSectionUseCases.GetPagedListAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }
}
