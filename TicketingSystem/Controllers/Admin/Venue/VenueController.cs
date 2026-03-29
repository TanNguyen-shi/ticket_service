using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.UseCases.Venue.Interfaces;
using Ticketing.Infrastructure.DTOs.Venue.Request;
using Ticketing.Infrastructure.Helpers.Interfaces;

namespace TicketingSystem.Controllers.Admin.Venue;

[Route("api/admin/venue")]
public class VenueController(IVenueUseCases venueUseCases, IUserHelper user) : BaseApiController
{
    /// <summary>
    /// Admin - Khai báo địa điểm tổ chức sự kiện
    /// </summary>
    /// <param name="request">Input : Thông tin cấu hình địa điểm</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Id record vừa tạo</returns>
    [HttpPost("insert")]
    public async Task<IActionResult> Insert([FromBody] VenueCreateRequest request, CancellationToken cancellationToken)
    {
        var result = await venueUseCases.InsertAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Cập nhật thông tin địa điểm tổ chức sự kiện
    /// </summary>
    /// <param name="request">Input : Thông tin địa điểm cần cập nhật</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Kết quả cập nhật địa điểm</returns>
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] VenueUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await venueUseCases.UpdateAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Xóa mềm địa điểm tổ chức sự kiện
    /// </summary>
    /// <param name="request">Input : Id địa điểm cần xóa</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Kết quả xóa địa điểm</returns>
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromBody] VenueDeleteRequest request, CancellationToken cancellationToken)
    {
        var result = await venueUseCases.DeleteAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Lấy chi tiết một địa điểm theo Id
    /// </summary>
    /// <param name="request">Input : Id địa điểm cần lấy chi tiết</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Thông tin chi tiết địa điểm</returns>
    [HttpGet("getbyid")]
    public async Task<IActionResult> GetById([FromQuery] VenueGetByIdRequest request, CancellationToken cancellationToken)
    {
        var result = await venueUseCases.GetByIdAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Lấy danh sách địa điểm có phân trang
    /// </summary>
    /// <param name="request">Input : Điều kiện lọc + phân trang</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Danh sách địa điểm theo trang</returns>
    [HttpGet("getpagedlist")]
    public async Task<IActionResult> GetPagedList([FromQuery] VenueGetPagedListRequest request, CancellationToken cancellationToken)
    {
        var result = await venueUseCases.GetPagedListAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("getall")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await venueUseCases.GetAllAsync(user.UserId,
            cancellationToken);
        return Ok(result);
    }
}