using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.UseCases.Auth.Interfaces;
using Ticketing.Domain.Domain.Auth.Interfaces;
using Ticketing.Infrastructure.DTOs.Auth.Request;

namespace TicketingSystem.Controllers.Admin.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthUseCases auth) : ControllerBase
{
    /// <summary>
    /// Auth - Lấy thông tin đăng nhập mẫu và sinh token
    /// </summary>
    /// <returns>Thông tin đăng nhập hiện tại</returns>
    [HttpPost("login")]
    public async Task<IActionResult> login([FromBody] AuthLoginRequest request)
    {
        var authInfo = await auth.LoginAsync(request);
        return Ok(authInfo);
    }
}