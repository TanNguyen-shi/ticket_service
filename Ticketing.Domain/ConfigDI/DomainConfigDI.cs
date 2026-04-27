using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ticketing.Domain.Domain.Auth;
using Ticketing.Domain.Domain.Auth.Interfaces;
using Ticketing.Domain.Domain.Event;
using Ticketing.Domain.Domain.Event.Interfaces;
using Ticketing.Domain.Domain.EventPublishLog;
using Ticketing.Domain.Domain.EventPublishLog.Interfaces;
using Ticketing.Domain.Domain.Venue;
using Ticketing.Domain.Domain.Venue.Interfaces;
using Ticketing.Domain.Domain.VenueSeat;
using Ticketing.Domain.Domain.VenueSeat.Interfaces;
using Ticketing.Domain.Domain.VenueSection;
using Ticketing.Domain.Domain.VenueSection.Interfaces;
using Ticketing.Domain.Domain.EventSeatInventory;
using Ticketing.Domain.Domain.EventSeatInventory.Interfaces;
using Ticketing.Domain.Domain.EventZone;
using Ticketing.Domain.Domain.EventZone.Interfaces;
using Ticketing.Domain.Domain.EventZonePrice;
using Ticketing.Domain.Domain.EventZonePrice.Interfaces;
using Ticketing.Domain.Domain.Ticket;
using Ticketing.Domain.Domain.Ticket.Interfaces;
using Ticketing.Domain.Domain.TicketOrder;
using Ticketing.Domain.Domain.TicketOrder.Interfaces;
using Ticketing.Domain.Domain.TicketOrderItem;
using Ticketing.Domain.Domain.TicketOrderItem.Interfaces;
using Ticketing.Domain.Domain.SysRole;
using Ticketing.Domain.Domain.SysRole.Interfaces;
using Ticketing.Domain.Domain.SysUser;
using Ticketing.Domain.Domain.SysUser.Interfaces;
using Ticketing.Domain.Domain.SysUserRole;
using Ticketing.Domain.Domain.SysUserRole.Interfaces;
using Ticketing.Domain.Domain.CustomerAuth;
using Ticketing.Domain.Domain.CustomerAuth.Interfaces;

namespace Ticketing.Domain.ConfigDI;

public static class DomainConfigDI
{
    public static IServiceCollection AddDomainServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IVenueDomainService, VenueDomainService>();
        services.AddScoped<IVenueSeatDomainService, VenueSeatDomainService>();
        services.AddScoped<IVenueSectionDomainService, VenueSectionDomainService>();
        services.AddScoped<IEventDomainService, EventDomainService>();
        services.AddScoped<IEventPublishLogDomainService, EventPublishLogDomainService>();
        services.AddScoped<IEventSeatInventoryDomainService, EventSeatInventoryDomainService>();
        services.AddScoped<IEventZoneDomainService, EventZoneDomainService>();
        services.AddScoped<IEventZonePriceDomainService, EventZonePriceDomainService>();
        services.AddScoped<ITicketOrderDomainService, TicketOrderDomainService>();
        services.AddScoped<ITicketOrderItemDomainService, TicketOrderItemDomainService>();
        services.AddScoped<ITicketDomainService, TicketDomainService>();
        services.AddScoped<ISysRoleDomainService, SysRoleDomainService>();
        services.AddScoped<ISysUserDomainService, SysUserDomainService>();
        services.AddScoped<ISysUserRoleDomainService, SysUserRoleDomainService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICustomerAuthDomainService, CustomerAuthDomainService>();

        return services;
    }
}