using Ticketing.Infrastructure.Configurations;

namespace Ticketing.Infrastructure.Repositories.EventPublishLog;

public interface IEventPublishLogUnitOfWork : IUnitOfWork
{
    IEventPublishLogRepository EventPublishLog { get; set; }
}

public class EventPublishLogUnitOfWork(
    IEventPublishLogRepository eventPublishLogRepository,
    DapperContext dapperContext,
    DapperContextAccessor contextAccessor)
    : UnitOfWork(dapperContext, contextAccessor), IEventPublishLogUnitOfWork
{
    public IEventPublishLogRepository EventPublishLog { get; set; } =
        eventPublishLogRepository ?? throw new ArgumentNullException(nameof(eventPublishLogRepository));
}

