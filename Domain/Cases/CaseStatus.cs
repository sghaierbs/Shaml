namespace Domain.Cases;

public enum CaseStatus
{
    // Legacy values retained for compatibility with existing persisted cases and workflows.
    New = 1,
    WaitingForSpecialist = 2,
    InProgress = 3,
    Completed = 4,
    Cancelled = 5,

    // Shaml case lifecycle. Do not renumber existing values.
    PendingAssignment = 6,
    UnderStudy = 7,
    PendingClassificationApproval = 8,
    Classified = 9,
    Closed = 10
}
