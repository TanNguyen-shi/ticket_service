using Ticketing.Application.DTOs.HealthCheck;
using Ticketing.Application.UseCases.Auth.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.Auth.Request;
using Ticketing.Infrastructure.DTOs.Auth.Response;
using Ticketing.Infrastructure.JWT.Interfaces;
using Ticketing.Infrastructure.JWT.Model;

namespace Ticketing.Application.UseCases.Auth;

public class AuthUseCases(
    IJWTTokenService jwtService
) : IAuthUseCases
{
    public async Task<ResponseMessage<AuthLoginDto>?> LoginAsync(AuthLoginRequest request, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<AuthLoginDto>();
        try
        {
            var user = new JwtUserInfo
            {
                user_id = 1,
                username = "admin",
                full_name = "System Admin",
                user_type = "admin",
                roles = new List<string> { "SUPER_ADMIN" }
            };

            var accessToken = jwtService.GenerateAccessToken(user);

            var response = new AuthLoginDto
            {
                access_token = accessToken,
                token_type = "Bearer",
                expires_in = jwtService.GetExpireInSeconds(),
                user = new AuthUserProfileDto
                {
                    user_id = user.user_id,
                    username = user.username,
                    full_name = user.full_name,
                    user_type = user.user_type,
                    status = "active",
                    roles = user.roles
                }
            };
            
            return new ResponseMessage<AuthLoginDto>().MessageSuccess(response, "Đăng nhập thành công");
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }
}