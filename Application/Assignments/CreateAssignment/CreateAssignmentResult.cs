namespace Application.Assignments.CreateAssignment;

public sealed record CreateAssignmentResult(
    Guid AssignmentId,
    Guid CaseId,
    string TaskCode);