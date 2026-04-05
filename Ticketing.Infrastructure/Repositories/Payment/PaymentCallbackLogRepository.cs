using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.Payment.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Persistence.Helpers;

namespace Ticketing.Infrastructure.Repositories.Payment;

/// <summary>
/// Interface for PaymentCallbackLog Repository
/// </summary>
public interface IPaymentCallbackLogRepository : IGenericRepository<PaymentCallbackLogEntity>
{
    Task<IEnumerable<TResult>> GetByPaymentIdAsync<TResult>(object param, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository for PaymentCallbackLog Entity
/// Handles all data access operations for payment callback log records
/// </summary>
public class PaymentCallbackLogRepository(
    IDapperProcedureHelper dapper,
    DapperContextAccessor contextAccessor)
    : Repository<PaymentCallbackLogEntity>(dapper, contextAccessor), IPaymentCallbackLogRepository
{
    protected override string Schema => "ticketing";
    protected override string TableName => "payment_callback_log";

    /// <summary>
    /// Get all callback logs by payment ID
    /// </summary>
    public async Task<IEnumerable<TResult>> GetByPaymentIdAsync<TResult>(
        object param,
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("getbypaymentid");
        return await _dapper.GetAllAsync<TResult>(
            Connection,
            spName,
            param?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }
}

