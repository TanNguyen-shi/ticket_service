using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.Idempotency.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Persistence.Helpers;

namespace Ticketing.Infrastructure.Repositories.Idempotency;

/// <summary>
/// Interface for Idempotency Request Repository
/// Giúp kiểm soát các yêu cầu lặp lại từ Client (Idempotency Pattern)
/// </summary>
public interface IIdempotencyRequestRepository : IGenericRepository<IdempotencyRequestEntity>
{
    /// <summary>
    /// Lấy idempotency request theo idempotency_key và request_type
    /// </summary>
    Task<TResult?> GetByKeyAsync<TResult>(object param, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lấy tất cả idempotency request theo user_id
    /// </summary>
    Task<IEnumerable<TResult>> GetByUserIdAsync<TResult>(object param, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository for Idempotency Request Entity
/// Quản lý các bản ghi idempotency để tránh xử lý lặp lại yêu cầu từ Client
/// </summary>
public class IdempotencyRequestRepository(
    IDapperProcedureHelper dapper,
    DapperContextAccessor contextAccessor)
    : Repository<IdempotencyRequestEntity>(dapper, contextAccessor), IIdempotencyRequestRepository
{
    protected override string Schema => "ticketing";
    protected override string TableName => "idempotency_request";

    /// <summary>
    /// Lấy idempotency request theo idempotency_key và request_type
    /// </summary>
    public async Task<TResult?> GetByKeyAsync<TResult>(
        object param,
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("getbyidempotency_key");
        return await _dapper.GetAsync<TResult>(
            Connection,
            spName,
            param?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }

    /// <summary>
    /// Lấy tất cả idempotency request theo user_id
    /// </summary>
    public async Task<IEnumerable<TResult>> GetByUserIdAsync<TResult>(
        object param,
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("getbyuserid");
        return await _dapper.GetAllAsync<TResult>(
            Connection,
            spName,
            param?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }
}

