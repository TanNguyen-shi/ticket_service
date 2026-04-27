using Microsoft.AspNetCore.Mvc;
using Ticketing.Application.UseCases.Client.Auth.Interfaces;
using Ticketing.Infrastructure.DTOs.Client.Auth.Request;

namespace TicketingSystem.Controllers.Client;

[ApiController]
[Route("api/client/auth")]
public class ClientAuthController(IClientAuthUseCases clientAuth) : ControllerBase
{
    /// <summary>
    /// Client - Đăng ký tài khoản khách hàng
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] ClientRegisterRequest request, CancellationToken cancellationToken = default)
    {
        var result = await clientAuth.RegisterAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Client - Đăng nhập tài khoản khách hàng
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] ClientLoginRequest request, CancellationToken cancellationToken = default)
    {
        var result = await clientAuth.LoginAsync(request, cancellationToken);
        return Ok(result);
    }
}
