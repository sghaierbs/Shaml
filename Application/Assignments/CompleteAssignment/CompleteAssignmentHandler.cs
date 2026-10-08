using System.Text.Json;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Assignments;
using Domain.Common.Outbox;

namespace Application.Assignments.CompleteAssignment;

public sealed class CompleteAssignmentHandler
{
    private readonly IAssignmentRepository _assignments;
    private readonly IOutboxRepository _outboxRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserContext _currentUser;

    public CompleteAssignmentHandler(
        IAssignmentRepository assignments,
        IOutboxRepository outboxRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserContext currentUser)
    {
        _assignments = assignments;
        _outboxRepository = outboxRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<CompleteAssignmentResult> HandleAsync(
        CompleteAssignmentCommand command,
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

        var completedAtUtc = DateTime.UtcNow;

        assignment.Complete(completedAtUtc);

        var payload = new AssignmentCompletedOutboxPayload(
            assignment.Id,
            assignment.CaseId,
            assignment.TaskCode,
            completedAtUtc);

        var outboxMessage = OutboxMessage.Create(
            OutboxMessageTypes.AssignmentCompleted,
            JsonSerializer.Serialize(payload),
            completedAtUtc);

        await _outboxRepository.AddAsync(
            outboxMessage,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new CompleteAssignmentResult(
            assignment.Id,
            assignment.Status,
            assignment.CompletedAtUtc!.Value);
    }
}