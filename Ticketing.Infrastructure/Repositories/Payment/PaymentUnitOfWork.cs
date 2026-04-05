using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Persistence.Helpers;

namespace Ticketing.Infrastructure.Repositories.Payment;

/// <summary>
/// Interface for Payment UnitOfWork
/// Groups PaymentTransaction and PaymentCallbackLog repositories together for transactional operations
/// </summary>
public interface IPaymentUnitOfWork : IUnitOfWork
{
    IPaymentTransactionRepository PaymentTransactionRepository { get; }
    IPaymentCallbackLogRepository PaymentCallbackLogRepository { get; }
}

/// <summary>
/// UnitOfWork for Payment module
/// Manages transaction scope for both PaymentTransaction and PaymentCallbackLog entities
/// </summary>
public class PaymentUnitOfWork(
    DapperContext context,
    DapperContextAccessor contextAccessor,
    IDapperProcedureHelper dapper) : UnitOfWork(context, contextAccessor), IPaymentUnitOfWork
{
    public IPaymentTransactionRepository PaymentTransactionRepository =>
        new PaymentTransactionRepository(dapper, contextAccessor);

    public IPaymentCallbackLogRepository PaymentCallbackLogRepository =>
        new PaymentCallbackLogRepository(dapper, contextAccessor);
}

