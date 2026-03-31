using Ticketing.Application.Model.DTOs;
using Ticketing.Domain.Domain.Auth.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.Auth.Request;
using Ticketing.Infrastructure.DTOs.Auth.Response;
using Ticketing.Infrastructure.DTOs.SysUser.Request;
using Ticketing.Infrastructure.DTOs.SysUser.Response;
using Ticketing.Infrastructure.Helpers.Interfaces;
using Ticketing.Infrastructure.JWT.Interfaces;
using Ticketing.Infrastructure.JWT.Model;
using Ticketing.Infrastructure.Repositories.SysAdmin;

namespace Ticketing.Domain.Domain.Auth;

public class AuthService(
    IJWTTokenService jwtTokenService,
    ISysAdminUnitOfWork sysAdminUnitOfWork,
    IPasswordHelper passwordHelper
) : IAuthService
{
    public async Task<ResponseMessage<AuthLoginDto>> Login(AuthLoginRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var systemUser = await sysAdminUnitOfWork.SysUser.GetByUserAsync<SysUserDto, SysUserGetByUserRequest>(new SysUserGetByUserRequest()
            {
                username = request.username
            }, cancellationToken);
            if (systemUser is null)
                return new ResponseMessage<AuthLoginDto>().MessageWarning("Tên đăng nhập hoặc mật khẩu không đúng");

            var hashPassword = passwordHelper.HashPassword(request.password);
            
            
            if (!passwordHelper.VerifyPassword(request.password, systemUser.password_hash))
                return new ResponseMessage<AuthLoginDto>().MessageWarning("Mật khẩu không chính xác, vui lòng thử lại sau.");

            var user = new JwtUserInfo
            {
                user_id = systemUser.user_id,
                username = systemUser.username,
                full_name = systemUser.full_name,
                roles = new List<string> { "admin" }
            };

            var accessToken = jwtTokenService.GenerateAccessToken(user);

            var response = new AuthLoginDto
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
                    status = systemUser.status,
                    roles = user.roles
                }
            };

            return new ResponseMessage<AuthLoginDto>().MessageSuccess(response, "Đăng nhập thành công");
        }
        catch (Exception e)
        {
            return new ResponseMessage<AuthLoginDto>().MessageError(e.Message);
        }
    }
    
}