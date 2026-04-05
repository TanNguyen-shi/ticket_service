using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Persistence.Helpers;
using Ticketing.Infrastructure.Repositories.EventSeatInventory;

namespace Ticketing.Infrastructure.Repositories.SeatHold;

/// <summary>
/// Interface for SeatHold UnitOfWork
/// Groups SeatHold and SeatHoldItem repositories together for transactional operations
/// </summary>
public interface ISeatHoldUnitOfWork : IUnitOfWork
{
    ISeatHoldRepository SeatHoldRepository { get; }
    ISeatHoldItemRepository SeatHoldItemRepository { get; }
    IEventSeatInventoryRepository EventSeatInventoryRepository { get; }
}

/// <summary>
/// UnitOfWork for SeatHold module
/// Manages transaction scope for both SeatHold and SeatHoldItem entities
/// </summary>
public class SeatHoldUnitOfWork(
    DapperContext context,
    DapperContextAccessor contextAccessor,
    IDapperProcedureHelper dapper) : UnitOfWork(context, contextAccessor), ISeatHoldUnitOfWork
{
    public ISeatHoldRepository SeatHoldRepository =>
        new SeatHoldRepository(dapper, contextAccessor);

    public ISeatHoldItemRepository SeatHoldItemRepository =>
        new SeatHoldItemRepository(dapper, contextAccessor);

    public IEventSeatInventoryRepository EventSeatInventoryRepository =>
        new EventSeatInventoryRepository(dapper, contextAccessor);
}