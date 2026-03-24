using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Ticketing.Infrastructure.Helpers.Interfaces;

namespace Ticketing.Infrastructure.Helpers.Impl;

public class UserHelper(IHttpContextAccessor httpContextAccessor) : IUserHelper
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;

    public long? UserId =>
        long.TryParse(User?.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

    public string Username =>
        User?.FindFirstValue(ClaimTypes.Name) ?? User?.Identity?.Name ?? string.Empty;

    public string FullName =>
        User?.FindFirstValue("full_name") ?? string.Empty;

    public string UserType =>
        User?.FindFirstValue("user_type") ?? string.Empty;

    public IReadOnlyList<string> Roles =>
        User?.FindAll(ClaimTypes.Role).Select(x => x.Value).Distinct().ToList()
        ?? new List<string>();
}