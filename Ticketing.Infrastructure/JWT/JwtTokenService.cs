using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Ticketing.Infrastructure.JWT.Interfaces;
using Ticketing.Infrastructure.JWT.Model;

namespace Ticketing.Infrastructure.JWT;

public class JwtTokenService(IOptions<JwtOptions> jwtOptions) : IJWTTokenService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public string GenerateAccessToken(JwtUserInfo user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.user_id.ToString()),
            new(ClaimTypes.Name, user.username),
            new("full_name", user.full_name),
            new("user_type", user.user_type)
        };

        foreach (var role in user.roles.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var expireAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpireMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expireAt,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public DateTime GetExpireAt()
    {
        return DateTime.UtcNow.AddMinutes(_jwtOptions.ExpireMinutes);
    }

    public int GetExpireInSeconds()
    {
        return _jwtOptions.ExpireMinutes * 60;
    }
}