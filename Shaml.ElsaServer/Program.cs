using Elsa.Extensions;
using Elsa.Persistence.EFCore;
using Elsa.Persistence.EFCore.Extensions;
using Elsa.Persistence.EFCore.Modules.Identity;
using Elsa.Persistence.EFCore.Modules.Management;
using Elsa.Persistence.EFCore.Modules.Runtime;
using Shaml.Workflows.Elsa;
using Shaml.Workflows.Elsa.Workflows;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("Elsa")
    ?? throw new InvalidOperationException(
        "Connection string 'Elsa' was not found.");

const string elsaSchema = "elsa";

builder.Services.AddElsa(elsa =>
{
    elsa.UseWorkflowManagement(management =>
        management.UseEntityFrameworkCore(ef =>
            ef.UsePostgreSql(
                connectionString,
                new ElsaDbContextOptions
                {
                    SchemaName = elsaSchema
                })));

    elsa.UseWorkflowRuntime(runtime =>
        runtime.UseEntityFrameworkCore(ef =>
            ef.UsePostgreSql(
                connectionString,
                new ElsaDbContextOptions
                {
                    SchemaName = elsaSchema
                })));

    elsa.UseIdentity(identity =>
    {
        identity.UseEntityFrameworkCore(ef =>
            ef.UsePostgreSql(
                connectionString,
                new ElsaDbContextOptions
                {
                    SchemaName = elsaSchema
                }));

        identity.UseDefaultAdmin(
            "admin",
            "admin123",
            "admin",
            ["*"]
        );
        identity.TokenOptions = options =>
        {
            options.SigningKey =
                "shaml-elsa-poc-signing-key-change-in-production-2026";
        };
    });
    
    // Elsa authentication
    elsa.UseDefaultAuthentication();

    // Elsa HTTP API
    elsa.UseWorkflowsApi();

    // Shaml workflow definitions
    elsa.AddWorkflow<CaseWorkflow>();
    elsa.AddWorkflow<ShamlPocWorkflow>();
});

builder.Services.AddShamlWorkflowServices();

var app = builder.Build();

app.MapWorkflowsApi();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/debug/endpoints", (
    IEnumerable<EndpointDataSource> sources) =>
{
    return sources
        .SelectMany(source => source.Endpoints)
        .OfType<RouteEndpoint>()
        .Select(endpoint => new
        {
            Route = endpoint.RoutePattern.RawText,
            Methods = endpoint.Metadata
                .GetMetadata<HttpMethodMetadata>()?
                .HttpMethods
        })
        .ToArray();
});

app.MapGet("/", () => "Shaml Elsa Server");

app.Run();