using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Assignments;

namespace Application.Assignments.ClaimAssignment;

public sealed class ClaimAssignmentHandler
{
    private readonly IAssignmentRepository _assignments;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserContext _currentUser;

    public ClaimAssignmentHandler(
        IAssignmentRepository assignments,
        IUnitOfWork unitOfWork,
        ICurrentUserContext currentUser)
    {
        _assignments = assignments;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ClaimAssignmentResult> HandleAsync(
        ClaimAssignmentCommand command,
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

        if (assignment.TargetType != AssignmentTargetType.RoleQueue)
        {
            throw new InvalidOperationException(
                "Only role queue assignments can be claimed.");
        }

        if (assignment.TargetRoleId != _currentUser.RoleId)
        {
            throw new UnauthorizedAccessException(
                "The assignment is not assigned to the active role.");
        }

        if (_currentUser.CenterId is null ||
            assignment.TargetCenterId != _currentUser.CenterId)
        {
            throw new UnauthorizedAccessException(
                "The assignment is not assigned to the active center.");
        }

        assignment.Claim(
            _currentUser.ActiveUserRoleId,
            DateTime.UtcNow);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new ClaimAssignmentResult(
            assignment.Id,
            assignment.Status,
            assignment.ClaimedByUserRoleId!.Value,
            assignment.ClaimedAtUtc!.Value);
    }
}