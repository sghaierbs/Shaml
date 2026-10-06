using System.Security.Claims;
using System.Text;
using Elsa.Extensions;
using Elsa.Persistence.EFCore;
using Elsa.Persistence.EFCore.Extensions;
using Elsa.Persistence.EFCore.Modules.Identity;
using Elsa.Persistence.EFCore.Modules.Management;
using Elsa.Persistence.EFCore.Modules.Runtime;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
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
    

    // Elsa HTTP API
    elsa.UseWorkflowsApi();

    // Shaml workflow definitions
    elsa.AddWorkflow<CaseWorkflow>();
    elsa.AddWorkflow<ShamlPocWorkflow>();
});


var shamlIssuer = builder.Configuration["Jwt:Issuer"]
                  ?? throw new InvalidOperationException("JWT issuer is not configured.");

var shamlAudience = builder.Configuration["Jwt:Audience"]
                    ?? throw new InvalidOperationException("JWT audience is not configured.");

var shamlSigningKey = builder.Configuration["Jwt:SigningKey"]
                      ?? throw new InvalidOperationException("JWT signing key is not configured.");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "ShamlBearer";
        options.DefaultChallengeScheme = "ShamlBearer";
    })
    .AddJwtBearer(
        "ShamlBearer",
        options =>
        {
            options.MapInboundClaims = false;

            options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = shamlIssuer,

                    ValidateAudience = true,
                    ValidAudience = shamlAudience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(shamlSigningKey)),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };

            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    var principal = context.Principal;

                    var hasElsaAccess =
                        string.Equals(
                            principal?.FindFirstValue("elsa_access"),
                            "true",
                            StringComparison.OrdinalIgnoreCase);

                    if (hasElsaAccess &&
                        principal?.Identity is ClaimsIdentity identity)
                    {
                        identity.AddClaim(
                            new Claim("permissions", "*"));
                    }

                    return Task.CompletedTask;
                }
            };
        });

builder.Services.AddAuthorization();

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

app.MapGet("/debug/shaml-auth", (HttpContext context) =>
    {
        return Results.Ok(new
        {
            authenticated = context.User.Identity?.IsAuthenticated,
            authenticationType = context.User.Identity?.AuthenticationType,
            claims = context.User.Claims.Select(x => new
            {
                x.Type,
                x.Value
            })
        });
    })
    .RequireAuthorization(policy =>
    {
        policy.AddAuthenticationSchemes("ShamlBearer");
        policy.RequireAuthenticatedUser();
    });

app.Run();