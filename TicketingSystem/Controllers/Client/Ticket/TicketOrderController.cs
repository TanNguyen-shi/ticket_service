using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.UseCases.TicketOrder.Interfaces;
using Ticketing.Infrastructure.DTOs.TicketOrder.Request;
using Ticketing.Infrastructure.Helpers.Interfaces;

namespace TicketingSystem.Controllers.Admin.Ticket;

[Route("api/admin/event/order")]
public class TicketOrderController(ITicketOrderUseCases ticketOrderUseCases, IUserHelper user) : BaseApiController
{
    /// <summary>
    /// Admin - Khai báo đơn đặt vé
    /// </summary>
    /// <param name="request">Input : Thông tin cấu hình đơn đặt vé</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Id record vừa tạo</returns>
    [HttpPost("insert")]
    public async Task<IActionResult> Insert([FromBody] TicketOrderCreateRequest request, CancellationToken cancellationToken)
    {
        var result = await ticketOrderUseCases.InsertAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Cập nhật thông tin đơn đặt vé
    /// </summary>
    /// <param name="request">Input : Thông tin đơn đặt vé cần cập nhật</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Kết quả cập nhật đơn đặt vé</returns>
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] TicketOrderUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await ticketOrderUseCases.UpdateAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Xóa đơn đặt vé
    /// </summary>
    /// <param name="request">Input : Id đơn đặt vé cần xóa</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Kết quả xóa đơn đặt vé</returns>
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromBody] TicketOrderDeleteRequest request, CancellationToken cancellationToken)
    {
        var result = await ticketOrderUseCases.DeleteAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Lấy chi tiết một đơn đặt vé theo Id
    /// </summary>
    /// <param name="request">Input : Id đơn đặt vé cần lấy chi tiết</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Thông tin chi tiết đơn đặt vé</returns>
    [HttpGet("getbyid")]
    public async Task<IActionResult> GetById([FromQuery] TicketOrderGetByIdRequest request, CancellationToken cancellationToken)
    {
        var result = await ticketOrderUseCases.GetByIdAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Lấy danh sách đơn đặt vé có phân trang
    /// </summary>
    /// <param name="request">Input : Điều kiện lọc + phân trang</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Danh sách đơn đặt vé theo trang</returns>
    [HttpGet("getpagedlist")]
    public async Task<IActionResult> GetPagedList([FromQuery] TicketOrderGetPagedListRequest request, CancellationToken cancellationToken)
    {
        var result = await ticketOrderUseCases.GetPagedListAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }
}
