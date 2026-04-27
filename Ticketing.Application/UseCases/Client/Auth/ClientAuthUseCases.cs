using Ticketing.Application.Model.DTOs;
using Ticketing.Application.UseCases.Client.Auth.Interfaces;
using Ticketing.Domain.Domain.CustomerAuth.Interfaces;
using Ticketing.Infrastructure.DTOs.Client.Auth.Request;
using Ticketing.Infrastructure.DTOs.Client.Auth.Response;

namespace Ticketing.Application.UseCases.Client.Auth;

public class ClientAuthUseCases(
    ICustomerAuthDomainService customerAuthDomainService
) : IClientAuthUseCases
{
    public async Task<ResponseMessage<ClientAuthDto>> RegisterAsync(
        ClientRegisterRequest request,
        CancellationToken ct = default)
    {
        try
        {
            return await customerAuthDomainService.RegisterAsync(request, ct);
        }
        catch (Exception e)
        {
            return new ResponseMessage<ClientAuthDto>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<ClientAuthDto>> LoginAsync(
        ClientLoginRequest request,
        CancellationToken ct = default)
    {
        try
        {
            return await customerAuthDomainService.LoginAsync(request, ct);
        }
        catch (Exception e)
        {
            return new ResponseMessage<ClientAuthDto>().MessageError(e.Message);
        }
    }
}
