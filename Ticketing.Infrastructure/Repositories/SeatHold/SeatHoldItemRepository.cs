using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.SeatHold.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Persistence.Helpers;

namespace Ticketing.Infrastructure.Repositories.SeatHold;

/// <summary>
/// Interface for SeatHoldItem Repository
/// </summary>
public interface ISeatHoldItemRepository : IGenericRepository<SeatHoldItemEntity>
{
    Task<IEnumerable<TResult>> GetByHoldIdAsync<TResult>(object param, CancellationToken cancellationToken = default);
    Task UpdateStatusByHoldIdAsync(object param, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository for SeatHoldItem Entity
/// Handles all data access operations for seat hold item records
/// </summary>
public class SeatHoldItemRepository(
    IDapperProcedureHelper dapper,
    DapperContextAccessor contextAccessor)
    : Repository<SeatHoldItemEntity>(dapper, contextAccessor), ISeatHoldItemRepository
{
    protected override string Schema => "ticketing";
    protected override string TableName => "seat_hold_item";

    public async Task<IEnumerable<TResult>> GetByHoldIdAsync<TResult>(
        object param,
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("getbyholdid");
        return await _dapper.GetAllAsync<TResult>(
            Connection,
            spName,
            param?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }

    public async Task UpdateStatusByHoldIdAsync(object param, CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("updatestatusbyholdid");
        await _dapper.ExecStoreToStringAsync(
            Connection,
            spName,
            param?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }
}

