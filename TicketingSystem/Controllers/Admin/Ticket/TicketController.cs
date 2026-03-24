using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.UseCases.Ticket.Interfaces;
using Ticketing.Infrastructure.DTOs.Ticket.Request;
using Ticketing.Infrastructure.Helpers.Interfaces;

namespace TicketingSystem.Controllers.Admin.Ticket;

[Route("api/admin/event/ticket")]
public class TicketController(ITicketUseCases ticketUseCases, IUserHelper user) : BaseApiController
{
    /// <summary>
    /// Admin - Khai báo vé
    /// </summary>
    /// <param name="request">Input : Thông tin cấu hình vé</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Id record vừa tạo</returns>
    [HttpPost("insert")]
    public async Task<IActionResult> Insert([FromBody] TicketCreateRequest request, CancellationToken cancellationToken)
    {
        var result = await ticketUseCases.InsertAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Cập nhật thông tin vé
    /// </summary>
    /// <param name="request">Input : Thông tin vé cần cập nhật</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Kết quả cập nhật vé</returns>
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] TicketUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await ticketUseCases.UpdateAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Xóa vé
    /// </summary>
    /// <param name="request">Input : Id vé cần xóa</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Kết quả xóa vé</returns>
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromBody] TicketDeleteRequest request, CancellationToken cancellationToken)
    {
        var result = await ticketUseCases.DeleteAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Lấy chi tiết một vé theo Id
    /// </summary>
    /// <param name="request">Input : Id vé cần lấy chi tiết</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Thông tin chi tiết vé</returns>
    [HttpGet("getbyid")]
    public async Task<IActionResult> GetById([FromQuery] TicketGetByIdRequest request, CancellationToken cancellationToken)
    {
        var result = await ticketUseCases.GetByIdAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Admin - Lấy danh sách vé có phân trang
    /// </summary>
    /// <param name="request">Input : Điều kiện lọc + phân trang</param>
    /// <param name="cancellationToken">Default -> dùng để build Transaction</param>
    /// <returns>Danh sách vé theo trang</returns>
    [HttpGet("getpagedlist")]
    public async Task<IActionResult> GetPagedList([FromQuery] TicketGetPagedListRequest request, CancellationToken cancellationToken)
    {
        var result = await ticketUseCases.GetPagedListAsync(
            request,
            user.UserId,
            cancellationToken);
        return Ok(result);
    }
}
