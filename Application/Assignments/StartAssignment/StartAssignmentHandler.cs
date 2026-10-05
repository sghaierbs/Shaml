using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Assignments;

namespace Application.Assignments.StartAssignment;

public sealed class StartAssignmentHandler
{
    private readonly IAssignmentRepository _assignments;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserContext _currentUser;

    public StartAssignmentHandler(
        IAssignmentRepository assignments,
        IUnitOfWork unitOfWork,
        ICurrentUserContext currentUser)
    {
        _assignments = assignments;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<StartAssignmentResult> HandleAsync(
        StartAssignmentCommand command,
        CancellationToken cancellationToken = default)
    {
        var assignment = await _assignments.GetByIdAsync(
            command.AssignmentId,
            cancellationToken);

        if (assignment is null)
        {
            throw new NotFoundException(
                $"Assignment '{command.AssignmentId}' was not found.");
        }

        switch (assignment.TargetType)
        {
            case AssignmentTargetType.RoleQueue:
                if (assignment.ClaimedByUserRoleId !=
                    _currentUser.ActiveUserRoleId)
                {
                    throw new UnauthorizedAccessException(
                        "The assignment was not claimed by the active user role.");
                }

                break;

            case AssignmentTargetType.UserRole:
                if (assignment.TargetUserRoleId !=
                    _currentUser.ActiveUserRoleId)
                {
                    throw new UnauthorizedAccessException(
                        "The assignment is not assigned to the active user role.");
                }

                break;

            case AssignmentTargetType.User:
                if (assignment.TargetUserId !=
                    _currentUser.UserId)
                {
                    throw new UnauthorizedAccessException(
                        "The assignment is not assigned to the current user.");
                }

                break;

            default:
                throw new InvalidOperationException(
                    "Unsupported assignment target type.");
        }

        var startedAtUtc = DateTime.UtcNow;

        assignment.Start(startedAtUtc);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new StartAssignmentResult(
            assignment.Id,
            assignment.Status,
            assignment.StartedAtUtc!.Value);
    }
}