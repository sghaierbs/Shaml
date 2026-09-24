using Domain.Identity;

namespace Shaml.Domain.Tests.Identity;

public class RoleTests
{
    [Fact]
    public void Create_ShouldCreateActiveRole()
    {
        var role = Role.Create(
            "center-director",
            "Center Director",
            PortalType.Internal,
            ScopeType.Center,
            true);

        Assert.NotEqual(Guid.Empty, role.Id);
        Assert.Equal("center-director", role.Code);
        Assert.Equal("Center Director", role.Name);
        Assert.Equal(PortalType.Internal, role.Portal);
        Assert.Equal(ScopeType.Center, role.ScopeType);
        Assert.True(role.IsSystem);
        Assert.True(role.IsActive);
    }

    [Fact]
    public void Deactivate_ShouldMakeRoleInactive()
    {
        var role = Role.Create(
            "center-director",
            "Center Director",
            PortalType.Internal,
            ScopeType.Center);

        role.Deactivate();

        Assert.False(role.IsActive);
    }
}