using Npgsql;
using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Persistence.Helpers;

namespace Ticketing.Infrastructure.Repositories;

public interface IRepository
{
    NpgsqlConnection? Connection { get; }
    NpgsqlTransaction? Transaction { get; }
}

public interface IGenericRepository<TEntity> : IRepository
    where TEntity : class
{
    Task<string?> InsertAsync<TParam>(TParam dto, CancellationToken cancellationToken = default);
    Task<string?> UpdateAsync<TParam>(TParam dto, CancellationToken cancellationToken = default);
    Task<string?> CheckExistAsync<TParam>(TParam dto, CancellationToken cancellationToken = default);
    Task<string?> DeleteAsync<TParam>(TParam dto, CancellationToken cancellationToken = default);

    Task<TResult?> GetAsync<TResult, TParam>(TParam param, CancellationToken cancellationToken = default);
    Task<IEnumerable<TResult>> GetPagedAsync<TResult, TParam>(TParam param, CancellationToken cancellationToken = default);

    Task<TResult?> GetAsync<TResult>(object param, CancellationToken cancellationToken = default);
    Task<IEnumerable<TResult>> GetPagedAsync<TResult>(object param, CancellationToken cancellationToken = default);
    Task<IEnumerable<TResult>> GetAllAsync<TResult>(object param, CancellationToken cancellationToken = default);
    Task<IEnumerable<TResult>> GetStatisticAsync<TResult>(object param, CancellationToken cancellationToken = default);

    // DataTable Export(object param);
    Task<IEnumerable<TResult>> ExportAsync<TResult>(object param, CancellationToken cancellationToken = default);
}

public abstract class Repository : IRepository
{
    protected abstract string Schema { get; }
    protected abstract string TableName { get; }

    protected readonly IDapperProcedureHelper _dapper;
    private readonly DapperContextAccessor _contextAccessor;

    protected Repository(
        IDapperProcedureHelper dapper,
        DapperContextAccessor contextAccessor)
    {
        _dapper = dapper ?? throw new ArgumentNullException(nameof(dapper));
        _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
    }

    protected string GetSpName(string action) => $"{Schema}.{TableName}_{action}";

    public NpgsqlConnection? Connection => _contextAccessor.Connection;
    public NpgsqlTransaction? Transaction => _contextAccessor.Transaction;
}

public abstract class Repository<TEntity>(
    IDapperProcedureHelper dapper,
    DapperContextAccessor contextAccessor) : Repository(dapper, contextAccessor), IGenericRepository<TEntity>
    where TEntity : class
{
    public virtual async Task<string?> InsertAsync<TParam>(
        TParam dto,
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("insert");

        return await _dapper.ExecStoreToStringAsync(
            Connection,
            spName,
            dto?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }

    public virtual async Task<string?> UpdateAsync<TParam>(
        TParam dto,
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("update");

        return await _dapper.ExecStoreToStringAsync(
            Connection,
            spName,
            dto?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }

    public virtual async Task<string?> CheckExistAsync<TParam>(
        TParam dto,
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("check");

        return await _dapper.GetAsync<string>(
            Connection,
            spName,
            dto?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }

    public virtual async Task<string?> DeleteAsync<TParam>(
        TParam dto,
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("delete");

        return await _dapper.ExecStoreToStringAsync(
            Connection,
            spName,
            dto?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }

    public virtual async Task<TResult?> GetAsync<TResult, TParam>(
        TParam param,
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("getbyid");

        return await _dapper.GetAsync<TResult>(
            Connection,
            spName,
            param?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }

    public virtual async Task<IEnumerable<TResult>> GetPagedAsync<TResult, TParam>(
        TParam param,
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("getpagedlist");

        return await _dapper.GetAllAsync<TResult>(
            Connection,
            spName,
            param?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }

    public virtual async Task<TResult?> GetAsync<TResult>(
        object param,
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("getbyid");

        return await _dapper.GetAsync<TResult>(
            Connection,
            spName,
            param?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }

    public virtual async Task<IEnumerable<TResult>> GetPagedAsync<TResult>(
        object param,
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("getpagedlist");

        return await _dapper.GetAllAsync<TResult>(
            Connection,
            spName,
            param?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }

    public virtual async Task<IEnumerable<TResult>> GetAllAsync<TResult>(
        object param,
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("getall");

        return await _dapper.GetAllAsync<TResult>(
            Connection,
            spName,
            param?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }

    public virtual async Task<IEnumerable<TResult>> GetStatisticAsync<TResult>(
        object param,
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("getstatistic");

        return await _dapper.GetAllAsync<TResult>(
            Connection,
            spName,
            param?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }

    // public virtual DataTable Export(object param)
    // {
    //     var spName = GetSpName("export");
    //
    //     return _dapper.ExecToDataTable(
    //         Connection,
    //         spName,
    //         param?.ToParameterArray(),
    //         30,
    //         Transaction);
    // }

    public virtual async Task<IEnumerable<TResult>> ExportAsync<TResult>(
        object param,
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("export");

        return await _dapper.GetAllAsync<TResult>(
            Connection,
            spName,
            param?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }
}