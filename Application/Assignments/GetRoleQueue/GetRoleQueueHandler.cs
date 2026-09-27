using Application.Common.Interfaces;

namespace Application.Assignments.GetRoleQueue;

public sealed class GetRoleQueueHandler
{
    private readonly IAssignmentRepository _assignments;
    private readonly ICurrentUserContext _currentUser;

    public GetRoleQueueHandler(
        IAssignmentRepository assignments,
        ICurrentUserContext currentUser)
    {
        _assignments = assignments;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyCollection<RoleQueueItemResult>> HandleAsync(
        CancellationToken cancellationToken = default)
    {
        if (_currentUser.CenterId is null)
        {
            return Array.Empty<RoleQueueItemResult>();
        }

        var assignments =
            await _assignments.GetOpenRoleQueueAsync(
                _currentUser.RoleId,
                _currentUser.CenterId.Value,
                cancellationToken);

        return assignments
            .Select(x => new RoleQueueItemResult(
                x.Id,
                x.CaseId,
                x.TaskCode,
                x.Status,
                x.CreatedAtUtc))
            .ToList();
    }
}