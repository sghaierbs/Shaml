using Application.Common.Interfaces;
using Domain.Identity;

namespace Application.Assignments.GetMyWork;

public sealed class GetMyWorkHandler
{
    private readonly IAssignmentRepository _assignments;
    private readonly ICurrentUserContext _currentUser;

    public GetMyWorkHandler(
        IAssignmentRepository assignments,
        ICurrentUserContext currentUser)
    {
        _assignments = assignments;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyCollection<MyWorkItemResult>> HandleAsync(
        CancellationToken cancellationToken = default)
    {
        var assignments =
            _currentUser.Portal == PortalType.Public
                ? await _assignments.GetWorkForUserAsync(
                    _currentUser.UserId,
                    cancellationToken)
                : await _assignments.GetWorkForUserRoleAsync(
                    _currentUser.ActiveUserRoleId,
                    cancellationToken);

        return assignments
            .Select(x => new MyWorkItemResult(
                x.Id,
                x.CaseId,
                x.TaskCode,
                x.Status,
                x.TargetType,
                x.CreatedAtUtc))
            .ToList();
    }
}