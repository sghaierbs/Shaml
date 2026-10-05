using Api.Authentication;
using Api.Endpoints;
using Application;
using Infrastructure;
using Elsa.Extensions;
using Infrastructure.Workflows.Elsa;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddElsaWorkflowRuntime(
    builder.Configuration);

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

// app.MapWorkflowsApi();

// app.UseWorkflows();

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