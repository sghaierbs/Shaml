using Domain.Cases.Events;
using Domain.Common;
using Shaml.Domain.Cases.Events;

namespace Domain.Cases;

public sealed class Case : AggregateRoot
{
    public string CaseNumber { get; private set; } = null!;
    
    public Guid CenterId { get; private set; }

    public CaseStatus Status { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private Case()
    {
    }

    private Case(
        Guid id,
        string caseNumber,
        Guid centerId,
        DateTime createdAtUtc) : base(id)
    {
        CaseNumber = caseNumber;
        CenterId = centerId;
        Status = CaseStatus.New;
        CreatedAtUtc = createdAtUtc;

        RaiseDomainEvent(
            new CaseCreatedEvent(
                Id,
                createdAtUtc));
    }

    public static Case Create(
        string caseNumber,
        Guid centerId)
    {
        if (string.IsNullOrWhiteSpace(caseNumber))
        {
            throw new ArgumentException(
                "Case number is required.",
                nameof(caseNumber));
        }

        if (centerId == Guid.Empty)
        {
            throw new ArgumentException(
                "CenterId is required.",
                nameof(centerId));
        }

        return new Case(
            Guid.NewGuid(),
            caseNumber.Trim(),
            centerId,
            DateTime.UtcNow);
    }
    
    public void Start()
    {
        if (Status != CaseStatus.WaitingForSpecialist)
        {
            throw new InvalidOperationException(
                $"Cannot start a case with status {Status}.");
        }

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
    
    public void WaitForSpecialist()
    {
        if (Status != CaseStatus.New)
        {
            throw new InvalidOperationException(
                $"Cannot send a case to the specialist queue with status {Status}.");
        }

        Status = CaseStatus.WaitingForSpecialist;
    }
}