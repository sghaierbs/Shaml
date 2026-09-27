namespace Domain.Cases;

public enum CaseStatus
{
    New = 1,
    WaitingForSpecialist = 2,
    InProgress = 3,
    Completed = 4,
    Cancelled = 5
}