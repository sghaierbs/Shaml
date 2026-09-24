using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Identity.AssignRole;
using Domain.Centers;
using Domain.Identity;
using Moq;

namespace Shaml.Application.Tests.Identity.AssignRole;

public class AssignRoleToUserHandlerTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IRoleRepository> _roles = new();
    private readonly Mock<IUserRoleRepository> _userRoles = new();
    private readonly Mock<ICenterRepository> _centers = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private AssignRoleToUserHandler CreateHandler()
    {
        return new AssignRoleToUserHandler(
            _users.Object,
            _roles.Object,
            _userRoles.Object,
            _centers.Object,
            _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_CenterRole_WithValidCenter_ShouldCreateAssignment()
    {
        var user = User.Create(
            "iam-user-001",
            "Ahmed");

        var role = Role.Create(
            "center-director",
            "Center Director",
            PortalType.Internal,
            ScopeType.Center);

        var center = Center.Create(
            "RYD-001",
            "Riyadh Center",
            "Riyadh",
            "Riyadh");

        _users
            .Setup(x => x.GetByIdAsync(
                user.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _roles
            .Setup(x => x.GetByIdAsync(
                role.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        _centers
            .Setup(x => x.GetByIdAsync(
                center.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(center);

        _userRoles
            .Setup(x => x.ExistsAsync(
                user.Id,
                role.Id,
                center.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = CreateHandler();

        var result = await handler.HandleAsync(
            new AssignRoleToUserCommand(
                user.Id,
                role.Id,
                center.Id));

        Assert.Equal(user.Id, result.UserId);
        Assert.Equal(role.Id, result.RoleId);
        Assert.Equal(center.Id, result.CenterId);

        _userRoles.Verify(
            x => x.AddAsync(
                It.Is<UserRole>(ur =>
                    ur.UserId == user.Id &&
                    ur.RoleId == role.Id &&
                    ur.CenterId == center.Id),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_CenterRole_WithoutCenter_ShouldThrowException()
    {
        var user = User.Create(
            "iam-user-001",
            "Ahmed");

        var role = Role.Create(
            "center-director",
            "Center Director",
            PortalType.Internal,
            ScopeType.Center);

        _users
            .Setup(x => x.GetByIdAsync(
                user.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _roles
            .Setup(x => x.GetByIdAsync(
                role.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleAsync(
                new AssignRoleToUserCommand(
                    user.Id,
                    role.Id)));

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_OrganizationRole_WithoutCenter_ShouldCreateAssignment()
    {
        var user = User.Create(
            "iam-user-001",
            "Ahmed");

        var role = Role.Create(
            "operations-supervisor",
            "Operations Supervisor",
            PortalType.Internal,
            ScopeType.Organization);

        _users
            .Setup(x => x.GetByIdAsync(
                user.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _roles
            .Setup(x => x.GetByIdAsync(
                role.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        _userRoles
            .Setup(x => x.ExistsAsync(
                user.Id,
                role.Id,
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = CreateHandler();

        var result = await handler.HandleAsync(
            new AssignRoleToUserCommand(
                user.Id,
                role.Id));

        Assert.Equal(user.Id, result.UserId);
        Assert.Equal(role.Id, result.RoleId);
        Assert.Null(result.CenterId);

        _userRoles.Verify(
            x => x.AddAsync(
                It.Is<UserRole>(ur =>
                    ur.UserId == user.Id &&
                    ur.RoleId == role.Id &&
                    ur.CenterId == null),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateAssignment_ShouldThrowException()
    {
        var user = User.Create(
            "iam-user-001",
            "Ahmed");

        var role = Role.Create(
            "operations-supervisor",
            "Operations Supervisor",
            PortalType.Internal,
            ScopeType.Organization);

        _users
            .Setup(x => x.GetByIdAsync(
                user.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _roles
            .Setup(x => x.GetByIdAsync(
                role.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        _userRoles
            .Setup(x => x.ExistsAsync(
                user.Id,
                role.Id,
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.HandleAsync(
                new AssignRoleToUserCommand(
                    user.Id,
                    role.Id)));

        _userRoles.Verify(
            x => x.AddAsync(
                It.IsAny<UserRole>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_UnknownUser_ShouldThrowException()
    {
        var userId = Guid.NewGuid();

        _users
            .Setup(x => x.GetByIdAsync(
                userId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.HandleAsync(
                new AssignRoleToUserCommand(
                    userId,
                    Guid.NewGuid())));

        _userRoles.Verify(
            x => x.AddAsync(
                It.IsAny<UserRole>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_UnknownRole_ShouldThrowException()
    {
        var user = User.Create(
            "iam-user-001",
            "Ahmed");

        var roleId = Guid.NewGuid();

        _users
            .Setup(x => x.GetByIdAsync(
                user.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _roles
            .Setup(x => x.GetByIdAsync(
                roleId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Role?)null);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.HandleAsync(
                new AssignRoleToUserCommand(
                    user.Id,
                    roleId)));

        _userRoles.Verify(
            x => x.AddAsync(
                It.IsAny<UserRole>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_InactiveRole_ShouldThrowException()
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

        _users
            .Setup(x => x.GetByIdAsync(
                user.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _roles
            .Setup(x => x.GetByIdAsync(
                role.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleAsync(
                new AssignRoleToUserCommand(
                    user.Id,
                    role.Id,
                    Guid.NewGuid())));

        _userRoles.Verify(
            x => x.AddAsync(
                It.IsAny<UserRole>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_CenterRole_WithUnknownCenter_ShouldThrowException()
    {
        var user = User.Create(
            "iam-user-001",
            "Ahmed");

        var role = Role.Create(
            "specialist",
            "Specialist",
            PortalType.Internal,
            ScopeType.Center);

        var centerId = Guid.NewGuid();

        _users
            .Setup(x => x.GetByIdAsync(
                user.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _roles
            .Setup(x => x.GetByIdAsync(
                role.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        _centers
            .Setup(x => x.GetByIdAsync(
                centerId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Center?)null);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.HandleAsync(
                new AssignRoleToUserCommand(
                    user.Id,
                    role.Id,
                    centerId)));

        _userRoles.Verify(
            x => x.AddAsync(
                It.IsAny<UserRole>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}