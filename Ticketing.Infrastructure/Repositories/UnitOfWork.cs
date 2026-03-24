using System.Data;
using Ticketing.Infrastructure.Configurations;

namespace Ticketing.Infrastructure.Repositories;

public interface IUnitOfWork : IDisposable
{
    Task OpenAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitAsync(bool isCloseConnection = true, CancellationToken cancellationToken = default);
    Task RollbackAsync(bool isCloseConnection = true, CancellationToken cancellationToken = default);
    Task CloseAsync();
}

public abstract class UnitOfWork(DapperContext context, DapperContextAccessor contextAccessor) : IUnitOfWork
{
    public async Task OpenAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("OpenAsync - ContextAccessor: " + contextAccessor.GetHashCode());

        if (contextAccessor.Connection != null)
            return;

        var connection = context.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        contextAccessor.SetConnection(connection);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("BeginTransactionAsync - ContextAccessor: " + contextAccessor.GetHashCode());
        var connection = contextAccessor.Connection;
        if (connection == null || connection.State == ConnectionState.Closed)
        {
            connection = context.CreateConnection();
            await connection.OpenAsync(cancellationToken);
            contextAccessor.SetConnection(connection);
        }

        var transaction = await connection.BeginTransactionAsync(cancellationToken);
        contextAccessor.SetTransaction(transaction);
    }

    public async Task CommitAsync(bool isCloseConnection = true, CancellationToken cancellationToken = default)
    {
        if (contextAccessor.Transaction != null)
            await contextAccessor.Transaction.CommitAsync(cancellationToken);

        if (isCloseConnection)
            await CloseAsync();
    }

    public async Task RollbackAsync(bool isCloseConnection = true, CancellationToken cancellationToken = default)
    {
        if (contextAccessor.Transaction != null)
            await contextAccessor.Transaction.RollbackAsync(cancellationToken);

        if (isCloseConnection)
            await CloseAsync();
    }

    public async Task CloseAsync()
    {
        if (contextAccessor.Transaction != null)
            await contextAccessor.Transaction.DisposeAsync();

        if (contextAccessor.Connection != null)
        {
            await contextAccessor.Connection.CloseAsync();
            await contextAccessor.Connection.DisposeAsync();
        }

        contextAccessor.Clear();
    }

    public void Dispose()
    {
        CloseAsync().Wait();
    }
}