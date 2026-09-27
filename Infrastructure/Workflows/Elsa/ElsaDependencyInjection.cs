using Elsa.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Workflows.Elsa.Workflows;

namespace Infrastructure.Workflows.Elsa;

public static class ElsaDependencyInjection
{
    public static IServiceCollection AddShamlElsa(this IServiceCollection services)
    {
        services.AddElsa(elsa => { elsa.AddWorkflow<ShamlPocWorkflow>(); });

        return services;
    }
}