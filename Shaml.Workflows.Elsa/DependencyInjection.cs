using Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Shaml.Workflows.Elsa.Runtime;

namespace Shaml.Workflows.Elsa;

public static class DependencyInjection
{
    public static IServiceCollection AddShamlWorkflowServices(
        this IServiceCollection services)
    {
        services.AddScoped<ICaseWorkflowService, ElsaCaseWorkflowService>();

        return services;
    }
}