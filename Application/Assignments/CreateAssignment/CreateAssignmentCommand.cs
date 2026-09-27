using Domain.Assignments;

namespace Application.Assignments.CreateAssignment;

public sealed record CreateAssignmentCommand(
    Guid CaseId,
    string TaskCode,
    AssignmentTargetType TargetType,
    Guid? TargetRoleId = null,
    Guid? TargetCenterId = null,
    Guid? TargetUserRoleId = null,
    Guid? TargetUserId = null);