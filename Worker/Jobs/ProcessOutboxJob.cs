using Application.Common.Outbox;

namespace Shaml.Worker.Jobs;

public sealed class ProcessOutboxJob
{
    private readonly OutboxProcessor _outboxProcessor;
    private readonly ILogger<ProcessOutboxJob> _logger;

    public ProcessOutboxJob(
        OutboxProcessor outboxProcessor,
        ILogger<ProcessOutboxJob> logger)
    {
        _outboxProcessor = outboxProcessor;
        _logger = logger;
    }

    public async Task ExecuteAsync(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Starting Outbox processing at {Time}.",
            DateTimeOffset.Now);

        try
        {
            await _outboxProcessor.ProcessAsync(
                cancellationToken);

            _logger.LogInformation(
                "Outbox processing completed at {Time}.",
                DateTimeOffset.Now);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Outbox processing failed.");

            throw;
        }
    }
}