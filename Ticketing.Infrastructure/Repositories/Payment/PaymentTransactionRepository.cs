using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.Payment.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Persistence.Helpers;

namespace Ticketing.Infrastructure.Repositories.Payment;

/// <summary>
/// Interface for PaymentTransaction Repository
/// </summary>
public interface IPaymentTransactionRepository : IGenericRepository<PaymentTransactionEntity>
{
    Task<IEnumerable<TResult>> GetByOrderIdAsync<TResult>(object param, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository for PaymentTransaction Entity
/// Handles all data access operations for payment transaction records
/// </summary>
public class PaymentTransactionRepository(
    IDapperProcedureHelper dapper,
    DapperContextAccessor contextAccessor)
    : Repository<PaymentTransactionEntity>(dapper, contextAccessor), IPaymentTransactionRepository
{
    protected override string Schema => "ticketing";
    protected override string TableName => "payment_transaction";

    /// <summary>
    /// Get all payment transactions by order ID
    /// </summary>
    public async Task<IEnumerable<TResult>> GetByOrderIdAsync<TResult>(
        object param,
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("getbyorderid");
        return await _dapper.GetAllAsync<TResult>(
            Connection,
            spName,
            param?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }
}

