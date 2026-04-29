using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.SeatHold.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Persistence.Helpers;

namespace Ticketing.Infrastructure.Repositories.SeatHold;

/// <summary>
/// Interface for SeatHold Repository
/// </summary>
public interface ISeatHoldRepository : IGenericRepository<SeatHoldEntity>
{
    Task<IEnumerable<TResult>> GetByEventIdAsync<TResult>(object param, CancellationToken cancellationToken = default);
    Task<IEnumerable<TResult>> GetByUserIdAsync<TResult>(object param, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy danh sách hold_id của các phiên giữ chỗ đang active nhưng đã hết hạn.
    /// Dùng cho background job tự động nhả ghế.
    /// </summary>
    Task<IEnumerable<TResult>> GetExpiredActiveAsync<TResult>(CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository for SeatHold Entity
/// Handles all data access operations for seat hold records
/// </summary>
public class SeatHoldRepository(
    IDapperProcedureHelper dapper,
    DapperContextAccessor contextAccessor)
    : Repository<SeatHoldEntity>(dapper, contextAccessor), ISeatHoldRepository
{
    protected override string Schema => "ticketing";
    protected override string TableName => "seat_hold";

    /// <summary>
    /// Get all seat holds by event ID
    /// </summary>
    public async Task<IEnumerable<TResult>> GetByEventIdAsync<TResult>(
        object param,
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("getbyeventid");
        return await _dapper.GetAllAsync<TResult>(
            Connection,
            spName,
            param?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }

    /// <summary>
    /// Get all seat holds by user ID
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

    public async Task<IEnumerable<TResult>> GetExpiredActiveAsync<TResult>(CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("getexpiredactive");
        return await _dapper.GetAllAsync<TResult>(
            Connection,
            spName,
            parameters: null,
            30,
            Transaction,
            cancellationToken);
    }
}

