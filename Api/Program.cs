using Api.Authentication;
using Api.BackgroundServices;
using Api.Endpoints;
using Application;
using Application.Common.Outbox;
using Elsa.Extensions;
using Elsa.Persistence.EFCore;
using Elsa.Persistence.EFCore.Extensions;
using Elsa.Persistence.EFCore.Modules.Identity;
using Elsa.Persistence.EFCore.Modules.Management;
using Elsa.Persistence.EFCore.Modules.Runtime;
using Infrastructure;
using Shaml.Workflows.Elsa;
using Shaml.Workflows.Elsa.Activities;
using Shaml.Workflows.Elsa.Workflows;

var builder = WebApplication.CreateBuilder(args);

var elsaConnectionString =
    builder.Configuration.GetConnectionString("Elsa")
    ?? throw new InvalidOperationException(
        "Connection string 'Elsa' was not found.");

const string elsaSchema = "elsa";

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddElsa(elsa =>
{
    elsa.UseWorkflowManagement(management =>
        management.UseEntityFrameworkCore(ef =>
            ef.UsePostgreSql(
                elsaConnectionString,
                new ElsaDbContextOptions
                {
                    SchemaName = elsaSchema
                })));

    elsa.UseWorkflowRuntime(runtime =>
        runtime.UseEntityFrameworkCore(ef =>
            ef.UsePostgreSql(
                elsaConnectionString,
                new ElsaDbContextOptions
                {
                    SchemaName = elsaSchema
                })));

    elsa.UseIdentity(identity =>
    {
        identity.UseEntityFrameworkCore(ef =>
            ef.UsePostgreSql(
                elsaConnectionString,
                new ElsaDbContextOptions
                {
                    SchemaName = elsaSchema
                }));

        identity.UseDefaultAdmin(
            "admin",
            "admin123",
            "admin",
            ["*"]);

        identity.TokenOptions = options =>
        {
            options.SigningKey =
                "shaml-elsa-poc-signing-key-change-in-production-2026";
        };
    });

    elsa.UseWorkflowsApi();

    elsa.AddActivity<WaitForAssignmentCompleted>();
    elsa.AddActivity<CreateDirectorApprovalAssignment>();
    elsa.AddWorkflow<CaseWorkflow>();
    elsa.AddWorkflow<ShamlPocWorkflow>();
});

builder.Services.AddShamlWorkflowServices();
builder.Services.AddScoped<OutboxProcessor>();
builder.Services.AddHostedService<OutboxBackgroundService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(GetSchemaId);

    options.ResolveConflictingActions(
        apiDescriptions => apiDescriptions.First());
});

builder.Services.AddShamlAuthentication(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("VueDevelopment", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

static string GetSchemaId(Type type)
{
    if (!type.IsGenericType)
    {
        return type.FullName?
                   .Replace("+", ".")
               ?? type.Name;
    }

    var genericTypeName =
        type.GetGenericTypeDefinition()
            .FullName!
            .Split('`')[0]
            .Replace("+", ".");

    var genericArguments =
        string.Join(
            "_",
            type.GetGenericArguments()
                .Select(GetSchemaId));

    return $"{genericTypeName}_{genericArguments}";
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseCors("VueDevelopment");
}

app.UseAuthentication();
app.UseAuthorization();

// -------------------------------
// Shaml endpoints
// -------------------------------

app.MapUserEndpoints();
app.MapCenterEndpoints();
app.MapCurrentUserEndpoints();
app.MapAssignmentEndpoints();

// -------------------------------
// Elsa
// -------------------------------

app.MapWorkflowsApi();

// -------------------------------
// Swagger
// -------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "Shaml API v1");
    });
}

app.MapControllers();

app.Run();