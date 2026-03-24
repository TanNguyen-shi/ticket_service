using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.Auth.Request;
using Ticketing.Infrastructure.DTOs.Auth.Response;

namespace Ticketing.Domain.Domain.Auth.Interfaces;

public interface IAuthService
{
    Task<ResponseMessage<AuthLoginDto>> Login(AuthLoginRequest request);
}