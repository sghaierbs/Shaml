using Application.Common.Events;
using Domain.Identity;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Shaml.Infrastructure.Tests.Identity;

public class PermissionServiceTests
{
    private static ShamlDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ShamlDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var domainEventDispatcher = new Mock<IDomainEventDispatcher>();

        return new ShamlDbContext(
            options,
            domainEventDispatcher.Object);
    }

    [Fact]
    public async Task GetPermissionsAsync_RoleHasPermissions_ShouldReturnPermissionCodes()
    {
        await using var dbContext = CreateDbContext();

        var role = Role.Create(
            "specialist",
            "Specialist",
            PortalType.Internal,
            ScopeType.Center);

        var caseView = Permission.Create(
            "case.view",
            "View cases");

        var sessionView = Permission.Create(
            "session.view",
            "View sessions");

        var sessionSchedule = Permission.Create(
            "session.schedule",
            "Schedule sessions");

        dbContext.Roles.Add(role);

        dbContext.Permissions.AddRange(
            caseView,
            sessionView,
            sessionSchedule);

        dbContext.RolePermissions.AddRange(
            RolePermission.Create(role.Id, caseView.Id),
            RolePermission.Create(role.Id, sessionView.Id),
            RolePermission.Create(role.Id, sessionSchedule.Id));

        await dbContext.SaveChangesAsync();

        var service = new PermissionService(dbContext);

        var result = await service.GetPermissionsAsync(role.Id);

        Assert.Equal(3, result.Count);

        Assert.Contains("case.view", result);
        Assert.Contains("session.view", result);
        Assert.Contains("session.schedule", result);
    }

    [Fact]
    public async Task GetPermissionsAsync_RoleHasNoPermissions_ShouldReturnEmptyCollection()
    {
        await using var dbContext = CreateDbContext();

        var role = Role.Create(
            "specialist",
            "Specialist",
            PortalType.Internal,
            ScopeType.Center);

        dbContext.Roles.Add(role);

        await dbContext.SaveChangesAsync();

        var service = new PermissionService(dbContext);

        var result = await service.GetPermissionsAsync(role.Id);

        Assert.Empty(result);
    }

    [Fact]
    public async Task HasPermissionAsync_RoleHasPermission_ShouldReturnTrue()
    {
        await using var dbContext = CreateDbContext();

        var role = Role.Create(
            "center-director",
            "Center Director",
            PortalType.Internal,
            ScopeType.Center);

        var permission = Permission.Create(
            "center.manage",
            "Manage Shaml centers");

        dbContext.Roles.Add(role);
        dbContext.Permissions.Add(permission);

        dbContext.RolePermissions.Add(
            RolePermission.Create(
                role.Id,
                permission.Id));

        await dbContext.SaveChangesAsync();

        var service = new PermissionService(dbContext);

        var result = await service.HasPermissionAsync(
            role.Id,
            "center.manage");

        Assert.True(result);
    }

    [Fact]
    public async Task HasPermissionAsync_RoleDoesNotHavePermission_ShouldReturnFalse()
    {
        await using var dbContext = CreateDbContext();

        var role = Role.Create(
            "specialist",
            "Specialist",
            PortalType.Internal,
            ScopeType.Center);

        var permission = Permission.Create(
            "center.manage",
            "Manage Shaml centers");

        dbContext.Roles.Add(role);
        dbContext.Permissions.Add(permission);

        await dbContext.SaveChangesAsync();

        var service = new PermissionService(dbContext);

        var result = await service.HasPermissionAsync(
            role.Id,
            "center.manage");

        Assert.False(result);
    }

    [Fact]
    public async Task HasPermissionAsync_EmptyPermissionCode_ShouldReturnFalse()
    {
        await using var dbContext = CreateDbContext();

        var service = new PermissionService(dbContext);

        var result = await service.HasPermissionAsync(
            Guid.NewGuid(),
            " ");

        Assert.False(result);
    }
}