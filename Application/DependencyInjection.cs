using Microsoft.Extensions.DependencyInjection;
using Application.Cases.CreateCase;
using Application.Cases.EventHandlers;
using Application.Common.Events;
using Domain.Cases.Events;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<CreateCaseHandler>();
        
        services.AddScoped<
            IDomainEventHandler<CaseCreatedEvent>,CaseCreatedEventHandler>();
        
        return services;
    }
}