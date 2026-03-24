using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ticketing.Application.UseCases.Event;
using Ticketing.Application.UseCases.Event.Interfaces;
using Ticketing.Application.UseCases.EventPublishLog;
using Ticketing.Application.UseCases.EventPublishLog.Interfaces;
using Ticketing.Application.UseCases.Venue;
using Ticketing.Application.UseCases.Venue.Interfaces;
using Ticketing.Application.UseCases.VenueSeat;
using Ticketing.Application.UseCases.VenueSeat.Interfaces;
using Ticketing.Application.UseCases.VenueSection;
using Ticketing.Application.UseCases.VenueSection.Interfaces;
using Ticketing.Application.UseCases.EventSeatInventory;
using Ticketing.Application.UseCases.EventSeatInventory.Interfaces;
using Ticketing.Application.UseCases.EventZone;
using Ticketing.Application.UseCases.EventZone.Interfaces;
using Ticketing.Application.UseCases.Ticket;
using Ticketing.Application.UseCases.Ticket.Interfaces;
using Ticketing.Application.UseCases.TicketOrder;
using Ticketing.Application.UseCases.TicketOrder.Interfaces;
using Ticketing.Application.UseCases.TicketOrderItem;
using Ticketing.Application.UseCases.TicketOrderItem.Interfaces;
using Ticketing.Domain.Domain.Auth;
using Ticketing.Domain.Domain.Auth.Interfaces;

namespace Ticketing.Application.ConfigDI;

public static class UseCaseConfigureDI
{
    public static IServiceCollection AddUseCaseService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IVenueUseCases, VenueUseCases>();
        services.AddScoped<IVenueSeatUseCases, VenueSeatUseCases>();
        services.AddScoped<IVenueSectionUseCases, VenueSectionUseCases>();
        services.AddScoped<IEventUseCases, EventUseCases>();
        services.AddScoped<IEventPublishLogUseCases, EventPublishLogUseCases>();
        services.AddScoped<IEventSeatInventoryUseCases, EventSeatInventoryUseCases>();
        services.AddScoped<IEventZoneUseCases, EventZoneUseCases>();
        services.AddScoped<ITicketOrderUseCases, TicketOrderUseCases>();
        services.AddScoped<ITicketOrderItemUseCases, TicketOrderItemUseCases>();
        services.AddScoped<ITicketUseCases, TicketUseCases>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}