using Elsa.Extensions;
using Elsa.Persistence.EFCore.Extensions;
using Elsa.Persistence.EFCore.Modules.Identity;
using Elsa.Persistence.EFCore.Modules.Management;
using Elsa.Persistence.EFCore.Modules.Runtime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Workflows.Elsa;

public static class ElsaDependencyInjection
{
    public static IServiceCollection AddElsaWorkflowEngine(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("Elsa")
            ?? throw new InvalidOperationException(
                "Connection string 'Elsa' was not found.");

        var identityTokenSection =
            configuration.GetSection("Elsa:Identity:Tokens");

        var adminUserName =
            configuration["Elsa:Identity:Bootstrap:UserName"]
            ?? throw new InvalidOperationException(
                "Elsa:Identity:Bootstrap:UserName is required.");

        var adminPassword =
            configuration["Elsa:Identity:Bootstrap:Password"]
            ?? throw new InvalidOperationException(
                "Elsa:Identity:Bootstrap:Password is required.");

        services.AddElsa(elsa =>
        {
            // Workflow definitions.
            elsa.UseWorkflowManagement(management =>
                management.UseEntityFrameworkCore(ef =>
                    ef.UsePostgreSql(connectionString)));

            // Workflow instances.
            elsa.UseWorkflowRuntime(runtime =>
                runtime.UseEntityFrameworkCore(ef =>
                    ef.UsePostgreSql(connectionString)));

            // Elsa Identity - used by Elsa Studio / Elsa API.
            elsa.UseIdentity(identity =>
            {
                identity.TokenOptions += options =>
                    identityTokenSection.Bind(options);

                identity.UseEntityFrameworkCore(ef =>
                    ef.UsePostgreSql(connectionString));

                identity.UseDefaultAdmin(admin => admin
                    .WithAdminUserName(adminUserName)
                    .WithAdminPassword(adminPassword)
                    .WithAdminRoleName("admin")
                    .WithAdminRolePermissions(
                        new List<string> { "*" }));
            });

            // Elsa API JWT authentication.
            elsa.UseDefaultAuthentication();

            // Elsa REST APIs required by Studio.
            elsa.UseWorkflowsApi();

            // HTTP workflow activities.
            elsa.UseHttp();

            // Delay/timer/scheduling activities.
            elsa.UseScheduling();
        });

        return services;
    }
}