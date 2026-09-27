using Domain.Common;

namespace Domain.Assignments;

public sealed class Assignment : AggregateRoot
{
    
    public Guid ConcurrencyToken { get; private set; } = Guid.NewGuid();
    
    /**
     * Currently the assignment does not know anything about Elsa
     * Create assignment:
     *   case.review
     *   → Specialist / Riyadh center
     */
    private Assignment()
    {
    }

    private Assignment(
        Guid id,
        Guid caseId,
        string taskCode,
        AssignmentTargetType targetType,
        DateTime createdAtUtc)
        : base(id)
    {
        if (caseId == Guid.Empty)
            throw new ArgumentException(
                "CaseId cannot be empty.",
                nameof(caseId));

        if (string.IsNullOrWhiteSpace(taskCode))
            throw new ArgumentException(
                "TaskCode cannot be empty.",
                nameof(taskCode));

        CaseId = caseId;
        TaskCode = taskCode;
        TargetType = targetType;
        Status = AssignmentStatus.Open;
        CreatedAtUtc = createdAtUtc;
    }
    
    public void RefreshConcurrencyToken()
    {
        ConcurrencyToken = Guid.NewGuid();
    }
    
    public static Assignment CreateForRoleQueue(
        Guid caseId,
        string taskCode,
        Guid roleId,
        Guid centerId,
        DateTime createdAtUtc)
    {
        if (roleId == Guid.Empty)
            throw new ArgumentException(
                "RoleId cannot be empty.",
                nameof(roleId));

        if (centerId == Guid.Empty)
            throw new ArgumentException(
                "CenterId cannot be empty.",
                nameof(centerId));

        var assignment = new Assignment(
            Guid.NewGuid(),
            caseId,
            taskCode,
            AssignmentTargetType.RoleQueue,
            createdAtUtc);

        assignment.TargetRoleId = roleId;
        assignment.TargetCenterId = centerId;

        return assignment;
    }
    
    public static Assignment CreateForUserRole(
        Guid caseId,
        string taskCode,
        Guid userRoleId,
        DateTime createdAtUtc)
    {
        if (userRoleId == Guid.Empty)
            throw new ArgumentException(
                "UserRoleId cannot be empty.",
                nameof(userRoleId));

        var assignment = new Assignment(
            Guid.NewGuid(),
            caseId,
            taskCode,
            AssignmentTargetType.UserRole,
            createdAtUtc);

        assignment.TargetUserRoleId = userRoleId;

        return assignment;
    }
    
    public static Assignment CreateForUser(
        Guid caseId,
        string taskCode,
        Guid userId,
        DateTime createdAtUtc)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "UserId cannot be empty.",
                nameof(userId));

        var assignment = new Assignment(
            Guid.NewGuid(),
            caseId,
            taskCode,
            AssignmentTargetType.User,
            createdAtUtc);

        assignment.TargetUserId = userId;

        return assignment;
    }
    
    public void Claim(
        Guid userRoleId,
        DateTime claimedAtUtc)
    {
        if (TargetType != AssignmentTargetType.RoleQueue)
        {
            throw new InvalidOperationException(
                "Only role queue assignments can be claimed.");
        }

        if (Status != AssignmentStatus.Open)
        {
            throw new InvalidOperationException(
                "Only an open assignment can be claimed.");
        }

        if (userRoleId == Guid.Empty)
        {
            throw new ArgumentException(
                "UserRoleId cannot be empty.",
                nameof(userRoleId));
        }

        ClaimedByUserRoleId = userRoleId;
        ClaimedAtUtc = claimedAtUtc;
        Status = AssignmentStatus.Claimed;
    }
    
    public void Complete(DateTime completedAtUtc)
    {
        // Assignment for public user does not need claiming that why it should be Open Claimed.
        if (Status != AssignmentStatus.Open &&
            Status != AssignmentStatus.Claimed)
        {
            throw new InvalidOperationException(
                "Only an open or claimed assignment can be completed.");
        }

        Status = AssignmentStatus.Completed;
        CompletedAtUtc = completedAtUtc;
    }
    
    public void Cancel()
    {
        if (Status == AssignmentStatus.Completed)
        {
            throw new InvalidOperationException(
                "A completed assignment cannot be cancelled.");
        }

        if (Status == AssignmentStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "The assignment is already cancelled.");
        }

        Status = AssignmentStatus.Cancelled;
    }

    public Guid CaseId { get; private set; }

    public string TaskCode { get; private set; } = null!;

    public AssignmentTargetType TargetType { get; private set; }

    public AssignmentStatus Status { get; private set; }

    public Guid? TargetRoleId { get; private set; }

    public Guid? TargetCenterId { get; private set; }

    public Guid? TargetUserRoleId { get; private set; }

    public Guid? TargetUserId { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? ClaimedAtUtc { get; private set; }

    public DateTime? CompletedAtUtc { get; private set; }

    public Guid? ClaimedByUserRoleId { get; private set; }
}