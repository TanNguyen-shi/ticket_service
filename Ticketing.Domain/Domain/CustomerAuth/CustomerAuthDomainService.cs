using Ticketing.Application.Model.DTOs;
using Ticketing.Domain.Domain.CustomerAuth.Interfaces;
using Ticketing.Infrastructure.DTOs.Client.Auth.Request;
using Ticketing.Infrastructure.DTOs.Client.Auth.Response;
using Ticketing.Infrastructure.Entities.Customer.Response;
using Ticketing.Infrastructure.Helpers.Interfaces;
using Ticketing.Infrastructure.JWT.Interfaces;
using Ticketing.Infrastructure.JWT.Model;
using Ticketing.Infrastructure.Repositories.Customer;

namespace Ticketing.Domain.Domain.CustomerAuth;

public class CustomerAuthDomainService(
    ICustomerUnitOfWork customerUnitOfWork,
    IJWTTokenService jwtTokenService,
    IPasswordHelper passwordHelper
) : ICustomerAuthDomainService
{
    public async Task<ResponseMessage<ClientAuthDto>> RegisterAsync(
        ClientRegisterRequest request,
        CancellationToken ct = default)
    {
        try
        {
            // 1. Check username + email uniqueness
            var existsRaw = await customerUnitOfWork.Customer.CheckExistAsync(new
            {
                customer_id = 0L,
                username = request.username,
                email = request.email ?? string.Empty
            }, ct);

            if (existsRaw == "1")
                return new ResponseMessage<ClientAuthDto>().MessageWarning("Tên đăng nhập hoặc email đã tồn tại");

            // 2. Hash password
            var passwordHash = passwordHelper.HashPassword(request.password);

            // 3. Generate customer_code
            var customerCode = $"CUS-{Guid.NewGuid():N}".Substring(0, 16).ToUpper();

            // 4. Insert customer
            await customerUnitOfWork.OpenAsync(ct);
            await customerUnitOfWork.BeginTransactionAsync(ct);

            var customerId = await customerUnitOfWork.Customer.InsertAsync(new
            {
                customer_code = customerCode,
                username = request.username,
                email = request.email ?? string.Empty,
                phone = request.phone ?? string.Empty,
                password_hash = passwordHash,
                full_name = request.full_name,
                avatar_url = string.Empty,
                status = "active"
            }, ct);

            if (string.IsNullOrEmpty(customerId) || !long.TryParse(customerId, out var customerIdLong) || customerIdLong <= 0)
                throw new Exception("Không thể tạo tài khoản, vui lòng thử lại");

            await customerUnitOfWork.CommitAsync(cancellationToken: ct);

            // 5. Generate JWT token
            var jwtUser = new JwtUserInfo
            {
                user_id = customerIdLong,
                username = request.username,
                full_name = request.full_name,
                user_type = "customer",
                roles = new List<string> { "customer" }
            };

            var accessToken = jwtTokenService.GenerateAccessToken(jwtUser);

            var response = new ClientAuthDto
            {
                access_token = accessToken,
                token_type = "Bearer",
                expires_in = jwtTokenService.GetExpireInSeconds(),
                customer = new ClientProfileDto
                {
                    customer_id = customerIdLong,
                    username = request.username,
                    email = request.email,
                    phone = request.phone,
                    full_name = request.full_name,
                    status = "active"
                }
            };

            return new ResponseMessage<ClientAuthDto>().MessageSuccess(response, "Đăng ký thành công");
        }
        catch (Exception e)
        {
            try { await customerUnitOfWork.RollbackAsync(cancellationToken: ct); } catch { /* best effort */ }
            return new ResponseMessage<ClientAuthDto>().MessageError(e.Message);
        }
    }

    public async Task<ResponseMessage<ClientAuthDto>> LoginAsync(
        ClientLoginRequest request,
        CancellationToken ct = default)
    {
        try
        {
            // 1. Get customer by username (read-only, no transaction)
            await customerUnitOfWork.OpenAsync(ct);

            var customer = await customerUnitOfWork.Customer.GetByUsernameAsync<CustomerEntity>(new
            {
                username = request.username
            }, ct);

            if (customer is null)
                return new ResponseMessage<ClientAuthDto>().MessageWarning("Tên đăng nhập hoặc mật khẩu không đúng");

            // 2. Check if account is active
            if (customer.is_deleted || customer.status != "active")
                return new ResponseMessage<ClientAuthDto>().MessageWarning("Tài khoản bị khóa");

            // 3. Verify password
            if (!passwordHelper.VerifyPassword(request.password, customer.password_hash))
                return new ResponseMessage<ClientAuthDto>().MessageWarning("Mật khẩu không chính xác");

            // 4. Update last_login_at - best effort, don't fail login if this update fails
            try
            {
                await customerUnitOfWork.Customer.UpdateLastLoginAsync(
                    new { customer_id = customer.customer_id }, ct);
            }
            catch { /* best effort */ }

            // 5. Generate JWT token
            var jwtUser = new JwtUserInfo
            {
                user_id = customer.customer_id,
                username = customer.username,
                full_name = customer.full_name,
                user_type = "customer",
                roles = new List<string> { "customer" }
            };

            var accessToken = jwtTokenService.GenerateAccessToken(jwtUser);

            var response = new ClientAuthDto
            {
                access_token = accessToken,
                token_type = "Bearer",
                expires_in = jwtTokenService.GetExpireInSeconds(),
                customer = new ClientProfileDto
                {
                    customer_id = customer.customer_id,
                    username = customer.username,
                    email = customer.email,
                    phone = customer.phone,
                    full_name = customer.full_name,
                    status = customer.status
                }
            };

            return new ResponseMessage<ClientAuthDto>().MessageSuccess(response, "Đăng nhập thành công");
        }
        catch (Exception e)
        {
            return new ResponseMessage<ClientAuthDto>().MessageError(e.Message);
        }
    }
}
