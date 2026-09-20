using Domain.Cases.Events;
using Domain.Common;
using Shaml.Domain.Cases.Events;

namespace Domain.Cases;

public sealed class Case : AggregateRoot
{
    public string CaseNumber { get; private set; } = null!;

    public CaseStatus Status { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private Case()
    {
    }

    private Case(
        Guid id,
        string caseNumber,
        DateTime createdAtUtc) : base(id)
    {
        CaseNumber = caseNumber;
        Status = CaseStatus.New;
        CreatedAtUtc = createdAtUtc;

        RaiseDomainEvent(
            new CaseCreatedEvent(
                Id,
                createdAtUtc));
    }

    public static Case Create(string caseNumber)
    {
        if (string.IsNullOrWhiteSpace(caseNumber))
            throw new ArgumentException(
                "Case number is required.",
                nameof(caseNumber));

        var id = Guid.NewGuid();
        var now = DateTime.UtcNow;

        return new Case(
            id,
            caseNumber.Trim(),
            now);
    }
    
    public void Start()
    {
        if (Status != CaseStatus.New)
            throw new InvalidOperationException(
                $"Cannot start a case with status {Status}.");

        Status = CaseStatus.InProgress;

        RaiseDomainEvent(
            new CaseStartedEvent(
                Id,
                DateTime.UtcNow));
    }

    public void Complete()
    {
        if (Status != CaseStatus.InProgress)
            throw new InvalidOperationException(
                $"Cannot complete a case with status {Status}.");

        Status = CaseStatus.Completed;

        RaiseDomainEvent(
            new CaseCompletedEvent(
                Id,
                DateTime.UtcNow));
    }
}