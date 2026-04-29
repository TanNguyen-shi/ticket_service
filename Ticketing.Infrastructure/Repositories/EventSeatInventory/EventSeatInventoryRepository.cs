using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Entities.EventSeatInventory.Response;
using Ticketing.Infrastructure.Extensions;
using Ticketing.Infrastructure.Persistence.Helpers;

namespace Ticketing.Infrastructure.Repositories.EventSeatInventory;

public interface IEventSeatInventoryRepository : IGenericRepository<EventSeatInventoryEntity>
{
    Task<IEnumerable<TResult>> GetBySeatIds<TResult>(object param, CancellationToken cancellationToken = default);

    Task<string?> UpdateOrderAsync<TParam>(
        TParam dto,
        CancellationToken cancellationToken = default);

    Task<string?> UpdateHoldAsync<TParam>(
        TParam dto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Reset ghế từ "held" → "available" khi phiên giữ chỗ bị huỷ hoặc hết hạn.
    /// </summary>
    Task UpdateReleaseAsync<TParam>(TParam dto, CancellationToken cancellationToken = default);
}

public class EventSeatInventoryRepository(
    IDapperProcedureHelper dapper,
    DapperContextAccessor contextAccessor)
    : Repository<EventSeatInventoryEntity>(dapper, contextAccessor), IEventSeatInventoryRepository
{
    protected override string Schema => "ticketing";
    protected override string TableName => "event_seat_inventory";

    public async Task<IEnumerable<EventSeatInventoryEntity>> GetFeaturedAsync(
        EventSeatInventoryEntity request,
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("getbyseatids");
        return await _dapper.GetAllAsync<EventSeatInventoryEntity>(
            Connection,
            spName,
            request.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }

    public async Task<IEnumerable<TResult>> GetBySeatIds<TResult>(object param, CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("getbyseatids");
        return await _dapper.GetAllAsync<TResult>(
            Connection,
            spName,
            param?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }

    public async Task<string?> UpdateHoldAsync<TParam>(
        TParam dto,
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("update_hold");

        return await _dapper.ExecStoreToStringAsync(
            Connection,
            spName,
            dto?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }

    public async Task<string?> UpdateOrderAsync<TParam>(
        TParam dto,
        CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("updateorder");

        return await _dapper.ExecStoreToStringAsync(
            Connection,
            spName,
            dto?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }

    public async Task UpdateReleaseAsync<TParam>(TParam dto, CancellationToken cancellationToken = default)
    {
        var spName = GetSpName("update_release");

        await _dapper.ExecStoreToStringAsync(
            Connection,
            spName,
            dto?.ToParameterArray(),
            30,
            Transaction,
            cancellationToken);
    }
}