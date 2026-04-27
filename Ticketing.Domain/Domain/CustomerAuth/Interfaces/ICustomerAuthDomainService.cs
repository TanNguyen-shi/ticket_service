using Ticketing.Application.Model.DTOs;
using Ticketing.Infrastructure.DTOs.Client.Auth.Request;
using Ticketing.Infrastructure.DTOs.Client.Auth.Response;

namespace Ticketing.Domain.Domain.CustomerAuth.Interfaces;

public interface ICustomerAuthDomainService
{
    Task<ResponseMessage<ClientAuthDto>> RegisterAsync(ClientRegisterRequest request, CancellationToken ct = default);
    Task<ResponseMessage<ClientAuthDto>> LoginAsync(ClientLoginRequest request, CancellationToken ct = default);
}
