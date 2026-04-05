using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.UseCases.Event.Interfaces;
using Ticketing.Infrastructure.DTOs.Admin.Event.Request;
using Ticketing.Infrastructure.DTOs.Event.Request;
using Ticketing.Infrastructure.Helpers.Interfaces;

namespace TicketingSystem.Controllers.Admin.Event;

[Route("api/admin/event")]
public class EventController(IEventUseCases eventUseCases, IUserHelper user) : BaseApiController
{
    /// <summary>
    /// Admin - Khai báo sự kiện
    /// </summary>
    /// <param name="request">Input : Thông tin cấu hình sự kiện</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Id record vừa tạo</returns>
    [HttpPost("insert")]
    public async Task<IActionResult> Insert([FromBody] EventCreateRequest request, CancellationToken cancellationToken)
    {
        var result = await eventUseCases.InsertAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Cập nhật thông tin sự kiện
    /// </summary>
    /// <param name="request">Input : Thông tin sự kiện cần cập nhật</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Kết quả cập nhật sự kiện</returns>
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] EventUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await eventUseCases.UpdateAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Xóa mềm sự kiện
    /// </summary>
    /// <param name="request">Input : Id sự kiện cần xóa</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Kết quả xóa sự kiện</returns>
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromBody] EventDeleteRequest request, CancellationToken cancellationToken)
    {
        var result = await eventUseCases.DeleteAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Lấy chi tiết một sự kiện theo Id
    /// </summary>
    /// <param name="request">Input : Id sự kiện cần lấy chi tiết</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Thông tin chi tiết sự kiện</returns>
    [HttpGet("getbyid")]
    public async Task<IActionResult> GetById([FromQuery] EventGetByIdRequest request, CancellationToken cancellationToken)
    {
        var result = await eventUseCases.GetByIdAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Lấy danh sách sự kiện có phân trang
    /// </summary>
    /// <param name="request">Input : Điều kiện lọc + phân trang</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Danh sách sự kiện theo trang</returns>
    [HttpGet("getpagedlist")]
    public async Task<IActionResult> GetPagedList([FromQuery] EventGetPagedListRequest request, CancellationToken cancellationToken)
    {
        var result = await eventUseCases.GetPagedListAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }
}
