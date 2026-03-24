using Ticketing.Infrastructure.Configurations;
using Ticketing.Infrastructure.Repositories.Ticket;
using Ticketing.Infrastructure.Repositories.TicketOrder;
using Ticketing.Infrastructure.Repositories.TicketOrderItem;

namespace Ticketing.Infrastructure.Repositories.Ticketing;

public interface ITicketingUnitOfWork : IUnitOfWork
{
    ITicketOrderRepository TicketOrder { get; set; }
    ITicketOrderItemRepository TicketOrderItem { get; set; }
    ITicketRepository Ticket { get; set; }
}

public class TicketingUnitOfWork(
    ITicketOrderRepository ticketOrderRepository,
    ITicketOrderItemRepository ticketOrderItemRepository,
    ITicketRepository ticketRepository,
    DapperContext dapperContext,
    DapperContextAccessor contextAccessor)
    : UnitOfWork(dapperContext, contextAccessor), ITicketingUnitOfWork
{
    public ITicketOrderRepository TicketOrder { get; set; } =
        ticketOrderRepository ?? throw new ArgumentNullException(nameof(ticketOrderRepository));

    public ITicketOrderItemRepository TicketOrderItem { get; set; } =
        ticketOrderItemRepository ?? throw new ArgumentNullException(nameof(ticketOrderItemRepository));

    public ITicketRepository Ticket { get; set; } =
        ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
}

