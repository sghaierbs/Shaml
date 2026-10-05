using Application.Common.Outbox;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure;
using Infrastructure.Workflows.Elsa;
using Shaml.Worker.Jobs;

var builder = Host.CreateApplicationBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("ShamlDatabase")
    ?? throw new InvalidOperationException(
        "Connection string 'ShamlDatabase' was not found.");

builder.Services.AddInfrastructure(
    builder.Configuration);

builder.Services.AddElsaWorkflowRuntime(
    builder.Configuration);

builder.Services.AddScoped<OutboxProcessor>();

builder.Services.AddHangfire(configuration =>
{
    configuration.UsePostgreSqlStorage(options =>
        options.UseNpgsqlConnection(connectionString));
});

builder.Services.AddHangfireServer();

builder.Services.AddScoped<ProcessOutboxJob>();

var host = builder.Build();

//
// Register recurring Hangfire jobs using DI,
// not the static RecurringJob API.
//
using (var scope = host.Services.CreateScope())
{
    var recurringJobManager =
        scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

    recurringJobManager.AddOrUpdate<ProcessOutboxJob>(
        "process-outbox",
        job => job.ExecuteAsync(CancellationToken.None),
        "*/1 * * * *");
}

await host.RunAsync();