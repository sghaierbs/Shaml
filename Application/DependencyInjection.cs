using Microsoft.Extensions.DependencyInjection;
using Application.Cases.CreateCase;
using Application.Cases.EventHandlers;
using Application.Centers.CreateCenter;
using Application.Common.Events;
using Application.Identity.AssignRole;
using Application.Identity.CurrentUser;
using Application.Identity.GetUserRoles;
using Application.Identity.ProvisionUser;
using Application.Identity.SwitchRole;
using Domain.Cases.Events;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<CreateCaseHandler>();
        
        services.AddScoped<IDomainEventHandler<CaseCreatedEvent>,CaseCreatedEventHandler>();
        
        services.AddScoped<AssignRoleToUserHandler>();
        
        services.AddScoped<ProvisionUserHandler>();
        
        services.AddScoped<CreateCenterHandler>();
        
        services.AddScoped<ICurrentUserContextResolver, CurrentUserContextResolver>();
        
        services.AddScoped<GetUserRolesHandler>();
        
        services.AddScoped<SwitchRoleHandler>();
        
        return services;
    }
}