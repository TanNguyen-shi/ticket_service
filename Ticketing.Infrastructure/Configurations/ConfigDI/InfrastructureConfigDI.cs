using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ticketing.Infrastructure.Helpers.Impl;
using Ticketing.Infrastructure.Helpers.Interfaces;
using Ticketing.Infrastructure.JWT;
using Ticketing.Infrastructure.JWT.Interfaces;
using Ticketing.Infrastructure.Persistence.Helpers;
using Ticketing.Infrastructure.Repositories.Event;
using Ticketing.Infrastructure.Repositories.EventPublishLog;
using Ticketing.Infrastructure.Repositories.EventSeatInventory;
using Ticketing.Infrastructure.Repositories.EventZone;
using Ticketing.Infrastructure.Repositories.EventZonePrice;
using Ticketing.Infrastructure.Repositories.Venue;
using Ticketing.Infrastructure.Repositories.VenueSeat;
using Ticketing.Infrastructure.Repositories.VenueSection;
using Ticketing.Infrastructure.Repositories.Ticket;
using Ticketing.Infrastructure.Repositories.TicketOrder;
using Ticketing.Infrastructure.Repositories.TicketOrderItem;
using Ticketing.Infrastructure.Repositories.Ticketing;
using Ticketing.Infrastructure.Repositories.SysAdmin;
using Ticketing.Infrastructure.Repositories.SysUserRole;

namespace Ticketing.Infrastructure.Configurations.ConfigDI;

public static class InfrastructureConfigDi
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<DapperContext>();
        services.AddScoped<DapperContextAccessor>();
        services.AddHttpContextAccessor();
        services.AddScoped<IDapperProcedureHelper, DapperProcedureHelper>();

        //Venue
        services.AddScoped<IVenueRepository, VenueRepository>();
        services.AddScoped<IVenueUnitOfWork, VenueUnitOfWork>();

        //Venue Seat
        services.AddScoped<IVenueSeatRepository, VenueSeatRepository>();
        services.AddScoped<IVenueSeatUnitOfWork, VenueSeatUnitOfWork>();

        //Venue Section
        services.AddScoped<IVenueSectionRepository, VenueSectionRepository>();
        services.AddScoped<IVenueSectionUnitOfWork, VenueSectionUnitOfWork>();

        //Event
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IEventUnitOfWork, EventUnitOfWork>();

        //Event publish log
        services.AddScoped<IEventPublishLogRepository, EventPublishLogRepository>();
        services.AddScoped<IEventPublishLogUnitOfWork, EventPublishLogUnitOfWork>();

        //Event seat inventory
        services.AddScoped<IEventSeatInventoryRepository, EventSeatInventoryRepository>();
        services.AddScoped<IEventSeatInventoryUnitOfWork, EventSeatInventoryUnitOfWork>();

        //Event zone
        services.AddScoped<IEventZoneRepository, EventZoneRepository>();
        services.AddScoped<IEventZoneUnitOfWork, EventZoneUnitOfWork>();

        //Event zone price
        services.AddScoped<IEventZonePriceRepository, EventZonePriceRepository>();
        services.AddScoped<IEventZonePriceUnitOfWork, EventZonePriceUnitOfWork>();

        //Ticketing (shared unit of work)
        services.AddScoped<ITicketOrderRepository, TicketOrderRepository>();
        services.AddScoped<ITicketOrderItemRepository, TicketOrderItemRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<ITicketingUnitOfWork, TicketingUnitOfWork>();

        //System (shared unit of work)
        services.AddScoped<ISysRoleRepository, SysRoleRepository>();
        services.AddScoped<ISysUserRepository, SysUserRepository>();
        services.AddScoped<ISysUserRoleRepository, SysUserRoleRepository>();
        services.AddScoped<ISysAdminUnitOfWork, SysAdminUnitOfWork>();

        services.AddScoped<IJWTTokenService, JwtTokenService>();

        //Add DI UserHelper
        services.AddScoped<IUserHelper, UserHelper>();
        services.AddScoped<IPasswordHelper, PasswordHelper>();


        return services;
    }
}