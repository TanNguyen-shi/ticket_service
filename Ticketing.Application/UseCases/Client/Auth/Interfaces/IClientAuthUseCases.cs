using Ticketing.Application.Model.DTOs;
using Ticketing.Infrastructure.DTOs.Client.Auth.Request;
using Ticketing.Infrastructure.DTOs.Client.Auth.Response;

namespace Ticketing.Application.UseCases.Client.Auth.Interfaces;

public interface IClientAuthUseCases
{
    Task<ResponseMessage<ClientAuthDto>> RegisterAsync(ClientRegisterRequest request, CancellationToken ct = default);
    Task<ResponseMessage<ClientAuthDto>> LoginAsync(ClientLoginRequest request, CancellationToken ct = default);
}
