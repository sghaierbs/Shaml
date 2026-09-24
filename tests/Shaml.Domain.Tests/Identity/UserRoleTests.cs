using Domain.Identity;

namespace Shaml.Domain.Tests.Identity;

public class UserRoleTests
{
    [Fact]
    public void Create_CenterScopedRole_WithCenterId_ShouldSucceed()
    {
        var role = Role.Create(
            "center-director",
            "Center Director",
            PortalType.Internal,
            ScopeType.Center);

        var userId = Guid.NewGuid();
        var centerId = Guid.NewGuid();

        var userRole = UserRole.Create(
            userId,
            role,
            centerId);

        Assert.Equal(userId, userRole.UserId);
        Assert.Equal(role.Id, userRole.RoleId);
        Assert.Equal(centerId, userRole.CenterId);
    }

    [Fact]
    public void Create_CenterScopedRole_WithoutCenterId_ShouldThrowException()
    {
        var role = Role.Create(
            "center-director",
            "Center Director",
            PortalType.Internal,
            ScopeType.Center);

        Assert.Throws<ArgumentException>(() =>
            UserRole.Create(
                Guid.NewGuid(),
                role));
    }

    [Fact]
    public void Create_OrganizationScopedRole_WithoutCenterId_ShouldSucceed()
    {
        var role = Role.Create(
            "operations-supervisor",
            "Operations Supervisor",
            PortalType.Internal,
            ScopeType.Organization);

        var userRole = UserRole.Create(
            Guid.NewGuid(),
            role);

        Assert.Null(userRole.CenterId);
    }

    [Fact]
    public void Create_OrganizationScopedRole_WithCenterId_ShouldThrowException()
    {
        var role = Role.Create(
            "operations-supervisor",
            "Operations Supervisor",
            PortalType.Internal,
            ScopeType.Organization);

        Assert.Throws<ArgumentException>(() =>
            UserRole.Create(
                Guid.NewGuid(),
                role,
                Guid.NewGuid()));
    }
}