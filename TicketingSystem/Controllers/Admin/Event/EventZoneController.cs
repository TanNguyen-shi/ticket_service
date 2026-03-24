using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.UseCases.EventZone.Interfaces;
using Ticketing.Infrastructure.DTOs.EventZone.Request;
using Ticketing.Infrastructure.Helpers.Interfaces;

namespace TicketingSystem.Controllers.Admin.Event;

[Route("api/admin/event/zone")]
public class EventZoneController(IEventZoneUseCases eventZoneUseCases, IUserHelper user) : BaseApiController
{
    /// <summary>
    /// Admin - Khai báo vùng bán vé của sự kiện
    /// </summary>
    /// <param name="request">Input : Thông tin cấu hình vùng</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Id record vừa tạo</returns>
    [HttpPost("insert")]
    public async Task<IActionResult> Insert([FromBody] EventZoneCreateRequest request, CancellationToken cancellationToken)
    {
        var result = await eventZoneUseCases.InsertAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Cập nhật thông tin vùng bán vé của sự kiện
    /// </summary>
    /// <param name="request">Input : Thông tin vùng cần cập nhật</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Kết quả cập nhật vùng</returns>
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] EventZoneUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await eventZoneUseCases.UpdateAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Xóa mềm vùng bán vé của sự kiện
    /// </summary>
    /// <param name="request">Input : Id vùng cần xóa</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Kết quả xóa vùng</returns>
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromBody] EventZoneDeleteRequest request, CancellationToken cancellationToken)
    {
        var result = await eventZoneUseCases.DeleteAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Lấy chi tiết một vùng bán vé theo Id
    /// </summary>
    /// <param name="request">Input : Id vùng cần lấy chi tiết</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Thông tin chi tiết vùng</returns>
    [HttpGet("getbyid")]
    public async Task<IActionResult> GetById([FromQuery] EventZoneGetByIdRequest request, CancellationToken cancellationToken)
    {
        var result = await eventZoneUseCases.GetByIdAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Lấy danh sách vùng bán vé có phân trang
    /// </summary>
    /// <param name="request">Input : Điều kiện lọc + phân trang</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Danh sách vùng theo trang</returns>
    [HttpGet("getpagedlist")]
    public async Task<IActionResult> GetPagedList([FromQuery] EventZoneGetPagedListRequest request, CancellationToken cancellationToken)
    {
        var result = await eventZoneUseCases.GetPagedListAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }
}
