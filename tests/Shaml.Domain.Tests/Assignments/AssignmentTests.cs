using Domain.Assignments;

namespace Shaml.Domain.Tests.Assignments;

public sealed class AssignmentTests
{
    [Fact]
    public void CreateForRoleQueue_ShouldCreateOpenAssignment()
    {
        var caseId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var centerId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var assignment = Assignment.CreateForRoleQueue(
            caseId,
            "case.review",
            roleId,
            centerId,
            now);

        Assert.Equal(caseId, assignment.CaseId);
        Assert.Equal("case.review", assignment.TaskCode);
        Assert.Equal(
            AssignmentTargetType.RoleQueue,
            assignment.TargetType);

        Assert.Equal(roleId, assignment.TargetRoleId);
        Assert.Equal(centerId, assignment.TargetCenterId);

        Assert.Null(assignment.TargetUserRoleId);
        Assert.Null(assignment.TargetUserId);

        Assert.Equal(
            AssignmentStatus.Open,
            assignment.Status);
    }

    [Fact]
    public void Claim_OpenRoleQueueAssignment_ShouldClaimIt()
    {
        var assignment = Assignment.CreateForRoleQueue(
            Guid.NewGuid(),
            "case.review",
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow);

        var userRoleId = Guid.NewGuid();
        var claimedAt = DateTime.UtcNow;

        assignment.Claim(
            userRoleId,
            claimedAt);

        Assert.Equal(
            AssignmentStatus.Claimed,
            assignment.Status);

        Assert.Equal(
            userRoleId,
            assignment.ClaimedByUserRoleId);

        Assert.Equal(
            claimedAt,
            assignment.ClaimedAtUtc);
    }

    [Fact]
    public void Claim_AlreadyClaimedAssignment_ShouldFail()
    {
        var assignment = Assignment.CreateForRoleQueue(
            Guid.NewGuid(),
            "case.review",
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow);

        assignment.Claim(
            Guid.NewGuid(),
            DateTime.UtcNow);

        Assert.Throws<InvalidOperationException>(
            () => assignment.Claim(
                Guid.NewGuid(),
                DateTime.UtcNow));
    }

    [Fact]
    public void Claim_UserAssignment_ShouldFail()
    {
        var assignment = Assignment.CreateForUser(
            Guid.NewGuid(),
            "case.upload-additional-attachment",
            Guid.NewGuid(),
            DateTime.UtcNow);

        Assert.Throws<InvalidOperationException>(
            () => assignment.Claim(
                Guid.NewGuid(),
                DateTime.UtcNow));
    }

    [Fact]
    public void Complete_OpenUserAssignment_ShouldComplete()
    {
        var assignment = Assignment.CreateForUser(
            Guid.NewGuid(),
            "case.upload-additional-attachment",
            Guid.NewGuid(),
            DateTime.UtcNow);

        var completedAt = DateTime.UtcNow;

        assignment.Complete(completedAt);

        Assert.Equal(
            AssignmentStatus.Completed,
            assignment.Status);

        Assert.Equal(
            completedAt,
            assignment.CompletedAtUtc);
    }
}