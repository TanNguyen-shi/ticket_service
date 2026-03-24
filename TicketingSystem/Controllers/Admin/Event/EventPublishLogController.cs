using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.UseCases.EventPublishLog.Interfaces;
using Ticketing.Infrastructure.DTOs.EventPublishLog.Request;
using Ticketing.Infrastructure.Helpers.Interfaces;

namespace TicketingSystem.Controllers.Admin.Event;

[Route("api/admin/event_publish_log")]
public class EventPublishLogController(IEventPublishLogUseCases eventPublishLogUseCases, IUserHelper user) : BaseApiController
{
    /// <summary>
    /// Admin - Khai báo nhật ký phát hành sự kiện
    /// </summary>
    /// <param name="request">Input : Thông tin nhật ký phát hành</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Id record vừa tạo</returns>
    [HttpPost("insert")]
    public async Task<IActionResult> Insert([FromBody] EventPublishLogCreateRequest request, CancellationToken cancellationToken)
    {
        var result = await eventPublishLogUseCases.InsertAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Cập nhật nhật ký phát hành sự kiện
    /// </summary>
    /// <param name="request">Input : Thông tin nhật ký cần cập nhật</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Kết quả cập nhật nhật ký</returns>
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] EventPublishLogUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await eventPublishLogUseCases.UpdateAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Xóa nhật ký phát hành sự kiện
    /// </summary>
    /// <param name="request">Input : Id nhật ký cần xóa</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Kết quả xóa nhật ký</returns>
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromBody] EventPublishLogDeleteRequest request, CancellationToken cancellationToken)
    {
        var result = await eventPublishLogUseCases.DeleteAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Lấy chi tiết một nhật ký phát hành theo Id
    /// </summary>
    /// <param name="request">Input : Id nhật ký cần lấy chi tiết</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Thông tin chi tiết nhật ký phát hành</returns>
    [HttpGet("getbyid")]
    public async Task<IActionResult> GetById([FromQuery] EventPublishLogGetByIdRequest request, CancellationToken cancellationToken)
    {
        var result = await eventPublishLogUseCases.GetByIdAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Lấy danh sách nhật ký phát hành có phân trang
    /// </summary>
    /// <param name="request">Input : Điều kiện lọc + phân trang</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Danh sách nhật ký phát hành theo trang</returns>
    [HttpGet("getpagedlist")]
    public async Task<IActionResult> GetPagedList([FromQuery] EventPublishLogGetPagedListRequest request, CancellationToken cancellationToken)
    {
        var result = await eventPublishLogUseCases.GetPagedListAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }
}
