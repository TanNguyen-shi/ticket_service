using Ticketing.Application.Model.DTOs;
using Ticketing.Application.UseCases.Auth.Interfaces;
using Ticketing.Domain.Domain.Auth.Interfaces;
using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.Auth.Request;
using Ticketing.Infrastructure.DTOs.Auth.Response;

namespace Ticketing.Application.UseCases.Auth;

public class AuthUseCases(
    IAuthService authService
) : IAuthUseCases
{
    public async Task<ResponseMessage<AuthLoginDto>?> LoginAsync(AuthLoginRequest request, CancellationToken cancellationToken = default)
    {
        var result = new ResponseMessage<AuthLoginDto>();
        try
        {
            return await authService.Login(request, cancellationToken);
        }
        catch (Exception e)
        {
            return result.MessageError(e.Message);
        }
    }
}