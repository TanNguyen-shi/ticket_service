using System.Data;
using Dapper;
using Npgsql;
using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Helpers.Model;

namespace Ticketing.Infrastructure.Persistence.Helpers;

public class DapperProcedureHelper : IDapperProcedureHelper
{
    private readonly DapperContext _context;

    public DapperProcedureHelper(DapperContext context)
    {
        _context = context;
    }

    public void ExecStoreNonQuery(string store, object[]? parameters = null, int timeout = 30)
    {
        ExecStoreNonQuery(null, store, parameters, timeout, null);
    }

    public void ExecStoreNonQuery(
        NpgsqlConnection? db,
        string store,
        object[]? parameters = null,
        int timeout = 30,
        NpgsqlTransaction? dbTransaction = null)
    {
        ExecuteDb(
            db,
            dbTransaction,
            (conn, tran) =>
            {
                CallFunctionNonQuery(conn, tran, store, parameters, timeout);
                return 0;
            });
    }

    public async Task ExecStoreNonQueryAsync(
        string store,
        object[]? parameters = null,
        int timeout = 30,
        CancellationToken cancellationToken = default)
    {
        await ExecStoreNonQueryAsync(null, store, parameters, timeout, null, cancellationToken);
    }

    public async Task ExecStoreNonQueryAsync(
        NpgsqlConnection? db,
        string store,
        object[]? parameters = null,
        int timeout = 30,
        NpgsqlTransaction? dbTransaction = null,
        CancellationToken cancellationToken = default)
    {
        await ExecuteDbAsync(
            db,
            dbTransaction,
            cancellationToken,
            async (conn, tran) =>
            {
                await CallFunctionNonQueryAsync(conn, tran, store, parameters, timeout, cancellationToken);
                return 0;
            });
    }

    public T? ExecStoreScalar<T>(string store, object[]? parameters = null, int timeout = 30)
    {
        return ExecStoreScalar<T>(null, store, parameters, timeout, null);
    }

    public T? ExecStoreScalar<T>(
        NpgsqlConnection? db,
        string store,
        object[]? parameters = null,
        int timeout = 30,
        NpgsqlTransaction? dbTransaction = null)
    {
        return ExecuteDb(
            db,
            dbTransaction,
            (conn, tran) => CallFunctionScalar<T>(conn, tran, store, parameters, timeout));
    }

    public async Task<T?> ExecStoreScalarAsync<T>(
        string store,
        object[]? parameters = null,
        int timeout = 30,
        CancellationToken cancellationToken = default)
    {
        return await ExecStoreScalarAsync<T>(null, store, parameters, timeout, null, cancellationToken);
    }

    public async Task<T?> ExecStoreScalarAsync<T>(
        NpgsqlConnection? db,
        string store,
        object[]? parameters = null,
        int timeout = 30,
        NpgsqlTransaction? dbTransaction = null,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteDbAsync(
            db,
            dbTransaction,
            cancellationToken,
            (conn, tran) => CallFunctionScalarAsync<T>(
                conn, tran, store, parameters, timeout, cancellationToken));
    }

    public async Task<string?> ExecStoreToStringAsync(
        string store,
        object[]? parameters = null,
        int timeout = 30,
        CancellationToken cancellationToken = default)
    {
        return await ExecStoreToStringAsync(null, store, parameters, timeout, null, cancellationToken);
    }

    public async Task<string?> ExecStoreToStringAsync(
        NpgsqlConnection? db,
        string store,
        object[]? parameters = null,
        int timeout = 30,
        NpgsqlTransaction? dbTransaction = null,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteDbAsync(
            db,
            dbTransaction,
            cancellationToken,
            async (conn, tran) =>
            {
                var cursorName = await CallFunctionForCursorAsync(
                    conn,
                    tran,
                    store,
                    parameters,
                    timeout,
                    cancellationToken);

                if (string.IsNullOrWhiteSpace(cursorName))
                    return null;

                var sql = $@"FETCH ALL IN ""{cursorName}"";";

                return await conn.QueryFirstOrDefaultAsync<string>(
                    new CommandDefinition(
                        sql,
                        parameters: null,
                        commandType: CommandType.Text,
                        commandTimeout: timeout,
                        transaction: tran,
                        cancellationToken: cancellationToken));
            });
    }

    public T? Get<T>(string store, object[]? parameters = null, int timeout = 30)
    {
        return Get<T>(null, store, parameters, timeout, null);
    }

    public T? Get<T>(
        NpgsqlConnection? db,
        string store,
        object[]? parameters = null,
        int timeout = 30,
        NpgsqlTransaction? dbTransaction = null)
    {
        return ExecuteDb(
            db,
            dbTransaction,
            (conn, tran) =>
            {
                var cursorName = CallFunctionForCursor(conn, tran, store, parameters, timeout);
                if (string.IsNullOrWhiteSpace(cursorName))
                    return default;

                var sql = $@"FETCH ALL IN ""{cursorName}"";";
                return conn.QueryFirstOrDefault<T>(sql, transaction: tran);
            });
    }

    public List<T> GetAll<T>(string store, object[]? parameters = null, int timeout = 30)
    {
        return GetAll<T>(null, store, parameters, timeout, null);
    }

    public List<T> GetAll<T>(
        NpgsqlConnection? db,
        string store,
        object[]? parameters = null,
        int timeout = 30,
        NpgsqlTransaction? dbTransaction = null)
    {
        return ExecuteDb(
            db,
            dbTransaction,
            (conn, tran) =>
            {
                var cursorName = CallFunctionForCursor(conn, tran, store, parameters, timeout);
                if (string.IsNullOrWhiteSpace(cursorName))
                    return new List<T>();

                var sql = $@"FETCH ALL IN ""{cursorName}"";";
                return conn.Query<T>(sql, transaction: tran).ToList();
            });
    }

    public async Task<T?> GetAsync<T>(
        string store,
        object[]? parameters = null,
        int timeout = 30,
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<T>(null, store, parameters, timeout, null, cancellationToken);
    }

    public async Task<T?> GetAsync<T>(
        NpgsqlConnection? db,
        string store,
        object[]? parameters = null,
        int timeout = 30,
        NpgsqlTransaction? dbTransaction = null,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteDbAsync(
            db,
            dbTransaction,
            cancellationToken,
            async (conn, tran) =>
            {
                var cursorName = await CallFunctionForCursorAsync(
                    conn, tran, store, parameters, timeout, cancellationToken);

                if (string.IsNullOrWhiteSpace(cursorName))
                    return default;

                var sql = $@"FETCH ALL IN ""{cursorName}"";";
                return await conn.QueryFirstOrDefaultAsync<T>(
                    new CommandDefinition(
                        sql,
                        transaction: tran,
                        commandType: CommandType.Text,
                        commandTimeout: timeout,
                        cancellationToken: cancellationToken));
            });
    }

    public async Task<IEnumerable<T>> GetAllAsync<T>(
        string store,
        object[]? parameters = null,
        int timeout = 30,
        CancellationToken cancellationToken = default)
    {
        return await GetAllAsync<T>(null, store, parameters, timeout, null, cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAllAsync<T>(
        NpgsqlConnection? db,
        string store,
        object[]? parameters = null,
        int timeout = 30,
        NpgsqlTransaction? dbTransaction = null,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteDbAsync(
            db,
            dbTransaction,
            cancellationToken,
            async (conn, tran) =>
            {
                var cursorName = await CallFunctionForCursorAsync(
                    conn, tran, store, parameters, timeout, cancellationToken);

                if (string.IsNullOrWhiteSpace(cursorName))
                    return Enumerable.Empty<T>();

                var sql = $@"FETCH ALL IN ""{cursorName}"";";
                return await conn.QueryAsync<T>(
                    new CommandDefinition(
                        sql,
                        transaction: tran,
                        commandType: CommandType.Text,
                        commandTimeout: timeout,
                        cancellationToken: cancellationToken));
            });
    }

    public DynamicParameters BuildDynamicParameters(object[]? parameters)
    {
        var dynamicParams = new DynamicParameters();

        if (parameters == null || parameters.Length == 0)
            return dynamicParams;

        if (parameters.Length % 2 != 0)
            throw new ArgumentException("Parameters must be key/value pairs.");

        for (int i = 0; i < parameters.Length; i += 2)
        {
            var rawKey = parameters[i]?.ToString();
            if (string.IsNullOrWhiteSpace(rawKey))
                throw new ArgumentException("Parameter name is invalid.");

            var key = FormatParameter(rawKey);
            var value = parameters[i + 1];

            if (value is ProcedureParam p)
            {
                dynamicParams.Add(key, p.Value, p.DbType, p.Direction);
            }
            else
            {
                dynamicParams.Add(key, value);
            }
        }

        return dynamicParams;
    }

    private string? CallFunctionForCursor(
        NpgsqlConnection conn,
        NpgsqlTransaction? tran,
        string store,
        object[]? parameters,
        int timeout)
    {
        return CallFunctionScalar<string>(conn, tran, store, parameters, timeout);
    }

    private async Task<string?> CallFunctionForCursorAsync(
        NpgsqlConnection conn,
        NpgsqlTransaction? tran,
        string store,
        object[]? parameters,
        int timeout,
        CancellationToken cancellationToken)
    {
        return await CallFunctionScalarAsync<string>(
            conn, tran, store, parameters, timeout, cancellationToken);
    }

    private T? CallFunctionScalar<T>(
        NpgsqlConnection conn,
        NpgsqlTransaction? tran,
        string store,
        object[]? parameters,
        int timeout)
    {
        var dp = BuildDynamicParameters(parameters);
        var sql = BuildFunctionSelectSql(store, dp.ParameterNames);

        var value = conn.ExecuteScalar(
            sql,
            dp,
            transaction: tran,
            commandType: CommandType.Text,
            commandTimeout: timeout);

        return ConvertValue<T>(value);
    }

    private async Task<T?> CallFunctionScalarAsync<T>(
        NpgsqlConnection conn,
        NpgsqlTransaction? tran,
        string store,
        object[]? parameters,
        int timeout,
        CancellationToken cancellationToken)
    {
        var dp = BuildDynamicParameters(parameters);
        var sql = BuildFunctionSelectSql(store, dp.ParameterNames);

        var value = await conn.ExecuteScalarAsync(
            new CommandDefinition(
                sql,
                dp,
                transaction: tran,
                commandType: CommandType.Text,
                commandTimeout: timeout,
                cancellationToken: cancellationToken));

        return ConvertValue<T>(value);
    }

    private void CallFunctionNonQuery(
        NpgsqlConnection conn,
        NpgsqlTransaction? tran,
        string store,
        object[]? parameters,
        int timeout)
    {
        var dp = BuildDynamicParameters(parameters);
        var sql = BuildFunctionSelectSql(store, dp.ParameterNames);

        conn.Execute(
            sql,
            dp,
            transaction: tran,
            commandType: CommandType.Text,
            commandTimeout: timeout);
    }

    private async Task CallFunctionNonQueryAsync(
        NpgsqlConnection conn,
        NpgsqlTransaction? tran,
        string store,
        object[]? parameters,
        int timeout,
        CancellationToken cancellationToken)
    {
        var dp = BuildDynamicParameters(parameters);
        var sql = BuildFunctionSelectSql(store, dp.ParameterNames);

        await conn.ExecuteAsync(
            new CommandDefinition(
                sql,
                dp,
                transaction: tran,
                commandType: CommandType.Text,
                commandTimeout: timeout,
                cancellationToken: cancellationToken));
    }

    private static string BuildFunctionSelectSql(string functionName, IEnumerable<string> parameterNames)
    {
        var names = parameterNames.ToList();

        if (names.Count == 0)
            return $"select {functionName}();";

        // Named notation avoids positional mismatch
        var args = names.Select(x => $"{x} => @{x}");
        return $"select {functionName}({string.Join(", ", args)});";
    }

    private static T? ConvertValue<T>(object? value)
    {
        if (value == null || value == DBNull.Value)
            return default;

        if (value is T matched)
            return matched;

        var targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        return (T?)Convert.ChangeType(value, targetType);
    }

    private T ExecuteDb<T>(
        NpgsqlConnection? db,
        NpgsqlTransaction? dbTransaction,
        Func<NpgsqlConnection, NpgsqlTransaction?, T> action)
    {
        if (db == null && dbTransaction != null)
            throw new InvalidOperationException("dbTransaction cannot exist when db is null.");

        var connection = db ?? _context.CreateConnection();
        var createdConnection = db == null;
        var openedHere = false;

        NpgsqlTransaction? transaction = dbTransaction;
        var createdTransaction = dbTransaction == null;

        try
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
                openedHere = true;
            }

            if (createdTransaction)
                transaction = connection.BeginTransaction();

            var result = action(connection, transaction);

            if (createdTransaction && transaction?.Connection != null)
                transaction.Commit();

            return result;
        }
        catch
        {
            if (createdTransaction && transaction?.Connection != null)
                transaction.Rollback();
            throw;
        }
        finally
        {
            if (openedHere && connection.State != ConnectionState.Closed)
                connection.Close();

            if (createdConnection)
            {
                connection.Dispose();
                NpgsqlConnection.ClearPool(connection);
            }
        }
    }

    private async Task<T> ExecuteDbAsync<T>(
        NpgsqlConnection? db,
        NpgsqlTransaction? dbTransaction,
        CancellationToken cancellationToken,
        Func<NpgsqlConnection, NpgsqlTransaction?, Task<T>> action)
    {
        if (db == null && dbTransaction != null)
            throw new InvalidOperationException("dbTransaction cannot exist when db is null.");

        var connection = db ?? _context.CreateConnection();
        var createdConnection = db == null;
        var openedHere = false;

        NpgsqlTransaction? transaction = dbTransaction;
        var createdTransaction = dbTransaction == null;

        try
        {
            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync(cancellationToken);
                openedHere = true;
            }

            if (createdTransaction)
                transaction = await connection.BeginTransactionAsync(cancellationToken);

            var result = await action(connection, transaction);

            if (createdTransaction && transaction?.Connection != null)
                await transaction.CommitAsync(cancellationToken);

            return result;
        }
        catch
        {
            if (createdTransaction && transaction?.Connection != null)
                await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (openedHere && connection.State != ConnectionState.Closed)
                await connection.CloseAsync();

            if (createdConnection)
            {
                await connection.DisposeAsync();
                NpgsqlConnection.ClearPool(connection);
            }
        }
    }

    private static string FormatParameter(string name)
    {
        name = name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Parameter name is empty.");

        if (name.StartsWith("@"))
            name = name[1..];

        if (!name.StartsWith("p_", StringComparison.OrdinalIgnoreCase))
            name = "p_" + name;

        return name.ToLowerInvariant();
    }
}
