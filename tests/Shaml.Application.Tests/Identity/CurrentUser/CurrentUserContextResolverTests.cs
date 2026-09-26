using Application.Common.Interfaces;
using Application.Identity.CurrentUser;
using Domain.Identity;
using Moq;

namespace Shaml.Application.Tests.Identity.CurrentUser;

public class CurrentUserContextResolverTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IUserRoleRepository> _userRoles = new();
    private readonly Mock<IRoleRepository> _roles = new();

    private CurrentUserContextResolver CreateResolver()
    {
        return new CurrentUserContextResolver(
            _users.Object,
            _userRoles.Object,
            _roles.Object);
    }

    [Fact]
    public async Task Resolve_WithoutRequestedAssignment_ShouldUseDefaultAssignment()
    {
        var user = User.Create(
            "iam-user-001",
            "Ahmed");

        var publicRole = Role.Create(
            "public-user",
            "Public User",
            PortalType.Public,
            ScopeType.Self);

        var publicAssignment = UserRole.Create(
            user.Id,
            publicRole,
            centerId: null,
            isDefault: true);

        _users
            .Setup(x => x.GetByExternalIdAsync(
                user.ExternalId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRoles
            .Setup(x => x.GetByUserIdAsync(
                user.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserRole>
            {
                publicAssignment
            });

        _roles
            .Setup(x => x.GetByIdAsync(
                publicRole.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(publicRole);

        var resolver = CreateResolver();

        var result = await resolver.ResolveAsync(
            user.ExternalId);

        Assert.Equal(user.Id, result.UserId);
        Assert.Equal(user.ExternalId, result.ExternalId);

        Assert.Equal(
            publicAssignment.Id,
            result.ActiveUserRoleId);

        Assert.Equal(
            publicRole.Id,
            result.RoleId);

        Assert.Equal(
            PortalType.Public,
            result.Portal);

        Assert.Equal(
            ScopeType.Self,
            result.ScopeType);

        Assert.Null(result.CenterId);
    }

    [Fact]
    public async Task Resolve_WithRequestedOwnAssignment_ShouldUseRequestedAssignment()
    {
        var user = User.Create(
            "iam-user-001",
            "Ahmed");

        var specialistRole = Role.Create(
            "specialist",
            "Specialist",
            PortalType.Internal,
            ScopeType.Center);

        var centerId = Guid.NewGuid();

        var specialistAssignment = UserRole.Create(
            user.Id,
            specialistRole,
            centerId,
            isDefault: false);

        _users
            .Setup(x => x.GetByExternalIdAsync(
                user.ExternalId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRoles
            .Setup(x => x.GetByIdAsync(
                specialistAssignment.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(specialistAssignment);

        _roles
            .Setup(x => x.GetByIdAsync(
                specialistRole.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(specialistRole);

        var resolver = CreateResolver();

        var result = await resolver.ResolveAsync(
            user.ExternalId,
            specialistAssignment.Id);

        Assert.Equal(
            specialistAssignment.Id,
            result.ActiveUserRoleId);

        Assert.Equal(
            specialistRole.Id,
            result.RoleId);

        Assert.Equal(
            PortalType.Internal,
            result.Portal);

        Assert.Equal(
            ScopeType.Center,
            result.ScopeType);

        Assert.Equal(
            centerId,
            result.CenterId);
    }

    [Fact]
    public async Task Resolve_WithAnotherUsersAssignment_ShouldReject()
    {
        var authenticatedUser = User.Create(
            "iam-user-001",
            "Ahmed");

        var anotherUser = User.Create(
            "iam-user-002",
            "Mohammed");

        var specialistRole = Role.Create(
            "specialist",
            "Specialist",
            PortalType.Internal,
            ScopeType.Center);

        var anotherUsersAssignment = UserRole.Create(
            anotherUser.Id,
            specialistRole,
            Guid.NewGuid());

        _users
            .Setup(x => x.GetByExternalIdAsync(
                authenticatedUser.ExternalId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(authenticatedUser);

        _userRoles
            .Setup(x => x.GetByIdAsync(
                anotherUsersAssignment.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(anotherUsersAssignment);

        var resolver = CreateResolver();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            resolver.ResolveAsync(
                authenticatedUser.ExternalId,
                anotherUsersAssignment.Id));

        _roles.Verify(
            x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Resolve_WithUnknownAssignment_ShouldReject()
    {
        var user = User.Create(
            "iam-user-001",
            "Ahmed");

        var assignmentId = Guid.NewGuid();

        _users
            .Setup(x => x.GetByExternalIdAsync(
                user.ExternalId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRoles
            .Setup(x => x.GetByIdAsync(
                assignmentId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserRole?)null);

        var resolver = CreateResolver();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            resolver.ResolveAsync(
                user.ExternalId,
                assignmentId));
    }

    [Fact]
    public async Task Resolve_WithoutDefaultAssignment_ShouldReject()
    {
        var user = User.Create(
            "iam-user-001",
            "Ahmed");

        var specialistRole = Role.Create(
            "specialist",
            "Specialist",
            PortalType.Internal,
            ScopeType.Center);

        var assignment = UserRole.Create(
            user.Id,
            specialistRole,
            Guid.NewGuid(),
            isDefault: false);

        _users
            .Setup(x => x.GetByExternalIdAsync(
                user.ExternalId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRoles
            .Setup(x => x.GetByUserIdAsync(
                user.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserRole>
            {
                assignment
            });

        var resolver = CreateResolver();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            resolver.ResolveAsync(user.ExternalId));

        _roles.Verify(
            x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Resolve_WithInactiveRole_ShouldReject()
    {
        var user = User.Create(
            "iam-user-001",
            "Ahmed");

        var role = Role.Create(
            "specialist",
            "Specialist",
            PortalType.Internal,
            ScopeType.Center);

        role.Deactivate();

        var assignment = UserRole.Create(
            user.Id,
            role,
            Guid.NewGuid(),
            isDefault: true);

        _users
            .Setup(x => x.GetByExternalIdAsync(
                user.ExternalId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRoles
            .Setup(x => x.GetByUserIdAsync(
                user.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserRole>
            {
                assignment
            });

        _roles
            .Setup(x => x.GetByIdAsync(
                role.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        var resolver = CreateResolver();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            resolver.ResolveAsync(user.ExternalId));
    }

    [Fact]
    public async Task Resolve_WithUnknownExternalIdentity_ShouldReject()
    {
        const string externalId = "unknown-user";

        _users
            .Setup(x => x.GetByExternalIdAsync(
                externalId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var resolver = CreateResolver();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            resolver.ResolveAsync(externalId));

        _userRoles.Verify(
            x => x.GetByUserIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}