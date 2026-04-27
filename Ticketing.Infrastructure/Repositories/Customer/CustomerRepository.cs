using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.Customer.Response;
using Ticketing.Infrastructure.Persistence.Helpers;
using Ticketing.Infrastructure.Extensions;

namespace Ticketing.Infrastructure.Repositories.Customer;

/// <summary>
/// Interface for Customer Repository
/// </summary>
public interface ICustomerRepository : IGenericRepository<CustomerEntity>
{
    Task<TResult?> GetByUsernameAsync<TResult>(object param, CancellationToken cancellationToken = default);
    Task<string?> UpdateLastLoginAsync(object param, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository for Customer Entity
/// Handles all data access operations for customer records
/// </summary>
public class CustomerRepository(
    IDapperProcedureHelper dapper,
    DapperContextAccessor contextAccessor)
    : Repository<CustomerEntity>(dapper, contextAccessor), ICustomerRepository
{
    protected override string Schema => "ticketing";
    protected override string TableName => "customer";

    /// <summary>
    /// Get a customer by username
    /// </summary>
    public async Task<TResult?> GetByUsernameAsync<TResult>(
        object param,
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("getbyusername");
        return await _dapper.GetAsync<TResult>(
            Connection,
            spName,
            param?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }

    /// <summary>
    /// Update last login timestamp for a customer
    /// </summary>
    public async Task<string?> UpdateLastLoginAsync(
        object param,
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("updatelastlogin");
        return await _dapper.ExecStoreToStringAsync(
            Connection,
            spName,
            param?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }
}

/// <summary>
/// Interface for Customer UnitOfWork
/// </summary>
public interface ICustomerUnitOfWork : IUnitOfWork
{
    ICustomerRepository Customer { get; }
}

/// <summary>
/// UnitOfWork for Customer module
/// Manages transaction scope for Customer entity
/// </summary>
public class CustomerUnitOfWork(
    DapperContext context,
    DapperContextAccessor contextAccessor,
    IDapperProcedureHelper dapper) : UnitOfWork(context, contextAccessor), ICustomerUnitOfWork
{
    public ICustomerRepository Customer =>
        new CustomerRepository(dapper, contextAccessor);
}
