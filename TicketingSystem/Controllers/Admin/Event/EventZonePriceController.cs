using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.UseCases.EventZonePrice.Interfaces;
using Ticketing.Infrastructure.DTOs.EventZonePrice.Request;
using Ticketing.Infrastructure.Helpers.Interfaces;

namespace TicketingSystem.Controllers.Admin.Event;

[Route("api/admin/event/zone-price")]
public class EventZonePriceController(IEventZonePriceUseCases eventZonePriceUseCases, IUserHelper user) : BaseApiController
{
    /// <summary>
    /// Admin - Khai báo giá bán cho vùng sự kiện
    /// </summary>
    /// <param name="request">Input : Thông tin giá vùng sự kiện</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Id record vừa tạo</returns>
    [HttpPost("insert")]
    public async Task<IActionResult> Insert([FromBody] EventZonePriceCreateRequest request, CancellationToken cancellationToken)
    {
        var result = await eventZonePriceUseCases.InsertAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Cập nhật giá bán của vùng sự kiện
    /// </summary>
    /// <param name="request">Input : Thông tin giá cần cập nhật</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Kết quả cập nhật giá</returns>
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] EventZonePriceUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await eventZonePriceUseCases.UpdateAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Xóa mềm giá bán vùng sự kiện
    /// </summary>
    /// <param name="request">Input : Id giá vùng cần xóa</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Kết quả xóa giá vùng</returns>
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromBody] EventZonePriceDeleteRequest request, CancellationToken cancellationToken)
    {
        var result = await eventZonePriceUseCases.DeleteAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Lấy chi tiết một bản giá theo Id
    /// </summary>
    /// <param name="request">Input : Id bản giá cần lấy chi tiết</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Thông tin chi tiết bản giá</returns>
    [HttpGet("getbyid")]
    public async Task<IActionResult> GetById([FromQuery] EventZonePriceGetByIdRequest request, CancellationToken cancellationToken)
    {
        var result = await eventZonePriceUseCases.GetByIdAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Lấy danh sách giá bán vùng sự kiện có phân trang
    /// </summary>
    /// <param name="request">Input : Điều kiện lọc + phân trang</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Danh sách giá vùng theo trang</returns>
    [HttpGet("getpagedlist")]
    public async Task<IActionResult> GetPagedList([FromQuery] EventZonePriceGetPagedListRequest request, CancellationToken cancellationToken)
    {
        var result = await eventZonePriceUseCases.GetPagedListAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }
}

