using Ticketing.Infrastructure.DTOs;
using Ticketing.Infrastructure.DTOs.Auth.Request;
using Ticketing.Infrastructure.DTOs.Auth.Response;

namespace Ticketing.Application.UseCases.Auth.Interfaces;

public interface IAuthUseCases
{
    Task<ResponseMessage<AuthLoginDto>?> LoginAsync(AuthLoginRequest request, CancellationToken cancellationToken = default);
}