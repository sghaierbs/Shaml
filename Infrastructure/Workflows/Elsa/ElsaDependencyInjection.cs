using Application.Common.Interfaces;
using Elsa.Extensions;
using Elsa.Persistence.EFCore.Extensions;
using Elsa.Persistence.EFCore.Modules.Management;
using Elsa.Persistence.EFCore.Modules.Runtime;
using Infrastructure.Workflows.Elsa.Runtime;
using Infrastructure.Workflows.Elsa.Workflows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Workflows.Elsa;

public static class ElsaDependencyInjection
{
    public static IServiceCollection AddShamlElsa(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ShamlDatabase") ?? throw new InvalidOperationException("Connection string 'ShamlDatabase' was not found.");

        services.AddElsa(elsa =>
        {
            elsa.UseWorkflowManagement(management =>
            {
                management.UseEntityFrameworkCore(ef =>
                {
                    ef.UsePostgreSql(connectionString);

                    // POC / development only.
                    ef.RunMigrations = true;
                });
            });

            elsa.UseWorkflowRuntime(runtime =>
            {
                runtime.UseEntityFrameworkCore(ef =>
                {
                    ef.UsePostgreSql(connectionString);

                    // POC / development only.
                    ef.RunMigrations = true;
                });
            });

            elsa.AddWorkflow<ShamlPocWorkflow>();
            elsa.AddWorkflow<CaseWorkflow>();
        });
        
        services.AddScoped<ShamlWorkflowRuntime>();
        services.AddScoped<ICaseWorkflowService, ElsaCaseWorkflowService>();

        return services;
    }
}