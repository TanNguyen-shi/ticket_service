using Ticketing.Infrastructure.JWT.Model;

namespace Ticketing.Infrastructure.JWT.Interfaces;

public interface IJWTTokenService
{
    string GenerateAccessToken(JwtUserInfo user);
    DateTime GetExpireAt();
    int GetExpireInSeconds();
}