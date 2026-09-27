using Application.Common.Interfaces;
using Domain.Assignments;

namespace Application.Assignments.CreateAssignment;

public sealed class CreateAssignmentHandler
{
    private readonly IAssignmentRepository _assignments;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAssignmentHandler(
        IAssignmentRepository assignments,
        IUnitOfWork unitOfWork)
    {
        _assignments = assignments;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateAssignmentResult> HandleAsync(
        CreateAssignmentCommand command,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var assignment = command.TargetType switch
        {
            AssignmentTargetType.RoleQueue =>
                Assignment.CreateForRoleQueue(
                    command.CaseId,
                    command.TaskCode,
                    command.TargetRoleId
                        ?? throw new ArgumentException(
                            "TargetRoleId is required for a role queue."),
                    command.TargetCenterId
                        ?? throw new ArgumentException(
                            "TargetCenterId is required for a role queue."),
                    now),

            AssignmentTargetType.UserRole =>
                Assignment.CreateForUserRole(
                    command.CaseId,
                    command.TaskCode,
                    command.TargetUserRoleId
                        ?? throw new ArgumentException(
                            "TargetUserRoleId is required for a UserRole assignment."),
                    now),

            AssignmentTargetType.User =>
                Assignment.CreateForUser(
                    command.CaseId,
                    command.TaskCode,
                    command.TargetUserId
                        ?? throw new ArgumentException(
                            "TargetUserId is required for a user assignment."),
                    now),

            _ => throw new ArgumentOutOfRangeException(
                nameof(command.TargetType))
        };

        await _assignments.AddAsync(
            assignment,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new CreateAssignmentResult(
            assignment.Id,
            assignment.CaseId,
            assignment.TaskCode);
    }
}