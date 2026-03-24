using Ticketing.Domain.Domain.Auth.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.Auth.Request;
using Ticketing.Infrastructure.DTOs.Auth.Response;
using Ticketing.Infrastructure.JWT;
using Ticketing.Infrastructure.JWT.Interfaces;
using Ticketing.Infrastructure.JWT.Model;

namespace Ticketing.Domain.Domain.Auth;

public class AuthService(IJWTTokenService jwtTokenService) : IAuthService
{
    public async Task<ResponseMessage<AuthLoginDto>> Login(AuthLoginRequest request)
    {
        var response = new AuthLoginDto();
        try
        {
            var user = new JwtUserInfo();
            if (request.username == "tan.nguyenthien" && request.password == "tan@313214")
            {
                //Todo : Giả lập dữ liệu -> Sẽ bổ sung logic kiểm tra username/password sau 
                user = new JwtUserInfo
                {
                    user_id = 1,
                    username = "tan.nguyenthien",
                    full_name = "System Admin",
                    user_type = "admin",
                    roles = new List<string> { "SUPER_ADMIN" }
                };
            }

            var accessToken = jwtTokenService.GenerateAccessToken(user);

            response = new AuthLoginDto
            {
                access_token = accessToken,
                token_type = "Bearer",
                expires_in = jwtTokenService.GetExpireInSeconds(),
                user = new AuthUserProfileDto
                {
                    user_id = user.user_id,
                    username = user.username,
                    full_name = user.full_name,
                    user_type = user.user_type,
                    roles = user.roles
                }
            };

            return new ResponseMessage<AuthLoginDto>().MessageSuccess(response);
        }
        catch (Exception e)
        {
            return new ResponseMessage<AuthLoginDto>().MessageError(e.Message);
        }
    }
}