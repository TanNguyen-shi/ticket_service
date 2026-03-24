using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ticketing.Domain.Domain.Auth.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.Auth.Request;

namespace TicketingSystem.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService auth) : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> me()
    {
        var authInfo = await auth.Login(new AuthLoginRequest()
        {
            username = "tan.nguyenthien",
            password = "tan@313214"
        });
        return Ok(authInfo);
    }
}