using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.UseCases.EventSeatInventory.Interfaces;
using Ticketing.Infrastructure.DTOs.EventSeatInventory.Request;
using Ticketing.Infrastructure.Helpers.Interfaces;

namespace TicketingSystem.Controllers.Admin.Event;

[Route("api/admin/event/seat-inventory")]
public class EventSeatInventoryController(IEventSeatInventoryUseCases eventSeatInventoryUseCases, IUserHelper user) : BaseApiController
{
    /// <summary>
    /// Admin - Khai báo tồn kho ghế theo sự kiện
    /// </summary>
    /// <param name="request">Input : Thông tin cấu hình tồn kho ghế</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Id record vừa tạo</returns>
    [HttpPost("insert")]
    public async Task<IActionResult> Insert([FromBody] EventSeatInventoryCreateRequest request, CancellationToken cancellationToken)
    {
        var result = await eventSeatInventoryUseCases.InsertAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Cập nhật tồn kho ghế theo sự kiện
    /// </summary>
    /// <param name="request">Input : Thông tin tồn kho ghế cần cập nhật</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Kết quả cập nhật tồn kho ghế</returns>
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] EventSeatInventoryUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await eventSeatInventoryUseCases.UpdateAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Xóa bản ghi tồn kho ghế theo sự kiện
    /// </summary>
    /// <param name="request">Input : Id tồn kho ghế cần xóa</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Kết quả xóa tồn kho ghế</returns>
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromBody] EventSeatInventoryDeleteRequest request, CancellationToken cancellationToken)
    {
        var result = await eventSeatInventoryUseCases.DeleteAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Lấy chi tiết một bản ghi tồn kho ghế theo Id
    /// </summary>
    /// <param name="request">Input : Id tồn kho ghế cần lấy chi tiết</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Thông tin chi tiết tồn kho ghế</returns>
    [HttpGet("getbyid")]
    public async Task<IActionResult> GetById([FromQuery] EventSeatInventoryGetByIdRequest request, CancellationToken cancellationToken)
    {
        var result = await eventSeatInventoryUseCases.GetByIdAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Lấy danh sách tồn kho ghế có phân trang
    /// </summary>
    /// <param name="request">Input : Điều kiện lọc + phân trang</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Danh sách tồn kho ghế theo trang</returns>
    [HttpGet("getpagedlist")]
    public async Task<IActionResult> GetPagedList([FromQuery] EventSeatInventoryGetPagedListRequest request, CancellationToken cancellationToken)
    {
        var result = await eventSeatInventoryUseCases.GetPagedListAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }
}
