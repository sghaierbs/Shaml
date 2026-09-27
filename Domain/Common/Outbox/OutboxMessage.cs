namespace Domain.Common.Outbox;

public sealed class OutboxMessage
{
    private OutboxMessage()
    {
    }

    private OutboxMessage(
        Guid id,
        string type,
        string payload,
        DateTime occurredAtUtc)
    {
        Id = id;
        Type = type;
        Payload = payload;
        OccurredAtUtc = occurredAtUtc;
    }

    public Guid Id { get; private set; }

    public string Type { get; private set; } = null!;

    public string Payload { get; private set; } = null!;

    public DateTime OccurredAtUtc { get; private set; }

    public DateTime? ProcessedAtUtc { get; private set; }

    public string? Error { get; private set; }

    public static OutboxMessage Create(
        string type,
        string payload,
        DateTime occurredAtUtc)
    {
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException(
                "Outbox message type is required.",
                nameof(type));

        if (string.IsNullOrWhiteSpace(payload))
            throw new ArgumentException(
                "Outbox message payload is required.",
                nameof(payload));

        return new OutboxMessage(
            Guid.NewGuid(),
            type,
            payload,
            occurredAtUtc);
    }

    public void MarkProcessed(DateTime processedAtUtc)
    {
        ProcessedAtUtc = processedAtUtc;
        Error = null;
    }

    public void MarkFailed(string error)
    {
        Error = error;
    }
}