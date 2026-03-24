using System.Data;

namespace Ticketing.Infrastructure.Interfaces.Configurations;

public interface ISqlExecutor
{
    Task<IEnumerable<T>> QueryAsync<T>(
        string sql,
        object? param = null,
        CommandType commandType = CommandType.Text,
        CancellationToken cancellationToken = default);

    Task<T?> QueryFirstOrDefaultAsync<T>(
        string sql,
        object? param = null,
        CommandType commandType = CommandType.Text,
        CancellationToken cancellationToken = default);

    Task<int> ExecuteAsync(
        string sql,
        object? param = null,
        CommandType commandType = CommandType.Text,
        CancellationToken cancellationToken = default);
}