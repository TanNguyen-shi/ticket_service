using System.Data;
using Dapper;
using Npgsql;

namespace Ticketing.Infrastructure.Persistence.Helpers;

public interface IDapperProcedureHelper
{
    void ExecStoreNonQuery(
        string store,
        object[]? parameters = null,
        int timeout = 30);

    void ExecStoreNonQuery(
        NpgsqlConnection? db,
        string store,
        object[]? parameters = null,
        int timeout = 30,
        NpgsqlTransaction? dbTransaction = null);

    Task ExecStoreNonQueryAsync(
        string store,
        object[]? parameters = null,
        int timeout = 30,
        CancellationToken cancellationToken = default);

    Task ExecStoreNonQueryAsync(
        NpgsqlConnection? db,
        string store,
        object[]? parameters = null,
        int timeout = 30,
        NpgsqlTransaction? dbTransaction = null,
        CancellationToken cancellationToken = default);

    T? ExecStoreScalar<T>(
        string store,
        object[]? parameters = null,
        int timeout = 30);

    T? ExecStoreScalar<T>(
        NpgsqlConnection? db,
        string store,
        object[]? parameters = null,
        int timeout = 30,
        NpgsqlTransaction? dbTransaction = null);

    Task<T?> ExecStoreScalarAsync<T>(
        string store,
        object[]? parameters = null,
        int timeout = 30,
        CancellationToken cancellationToken = default);

    Task<T?> ExecStoreScalarAsync<T>(
        NpgsqlConnection? db,
        string store,
        object[]? parameters = null,
        int timeout = 30,
        NpgsqlTransaction? dbTransaction = null,
        CancellationToken cancellationToken = default);

    Task<string?> ExecStoreToStringAsync(
        string store,
        object[]? parameters = null,
        int timeout = 30,
        CancellationToken cancellationToken = default);

    Task<string?> ExecStoreToStringAsync(
        NpgsqlConnection? db,
        string store,
        object[]? parameters = null,
        int timeout = 30,
        NpgsqlTransaction? dbTransaction = null,
        CancellationToken cancellationToken = default);

    T? Get<T>(
        string store,
        object[]? parameters = null,
        int timeout = 30);

    T? Get<T>(
        NpgsqlConnection? db,
        string store,
        object[]? parameters = null,
        int timeout = 30,
        NpgsqlTransaction? dbTransaction = null);

    List<T> GetAll<T>(
        string store,
        object[]? parameters = null,
        int timeout = 30);

    List<T> GetAll<T>(
        NpgsqlConnection? db,
        string store,
        object[]? parameters = null,
        int timeout = 30,
        NpgsqlTransaction? dbTransaction = null);

    Task<T?> GetAsync<T>(
        string store,
        object[]? parameters = null,
        int timeout = 30,
        CancellationToken cancellationToken = default);

    Task<T?> GetAsync<T>(
        NpgsqlConnection? db,
        string store,
        object[]? parameters = null,
        int timeout = 30,
        NpgsqlTransaction? dbTransaction = null,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<T>> GetAllAsync<T>(
        string store,
        object[]? parameters = null,
        int timeout = 30,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<T>> GetAllAsync<T>(
        NpgsqlConnection? db,
        string store,
        object[]? parameters = null,
        int timeout = 30,
        NpgsqlTransaction? dbTransaction = null,
        CancellationToken cancellationToken = default);

    DynamicParameters BuildDynamicParameters(object[]? parameters);
}