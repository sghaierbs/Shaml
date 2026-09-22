using Application.Common.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Shaml.Infrastructure.Events;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("ShamlDatabase")
            ?? throw new InvalidOperationException(
                "Connection string 'ShamlDatabase' was not found.");

        services.AddDbContext<ShamlDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<ICaseRepository, CaseRepository>();
        
        services.AddScoped<IUnitOfWork>(
            provider => provider.GetRequiredService<ShamlDbContext>());
        
        services.AddScoped<IDomainEventDispatcher,DomainEventDispatcher>();
        
        services.AddScoped<ICenterRepository, CenterRepository>();

        return services;
    }
}