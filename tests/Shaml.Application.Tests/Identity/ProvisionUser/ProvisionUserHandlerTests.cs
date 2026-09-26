using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Identity.ProvisionUser;
using Domain.Identity;
using Moq;

namespace Shaml.Application.Tests.Identity.ProvisionUser;

public class ProvisionUserHandlerTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IRoleRepository> _roles = new();
    private readonly Mock<IUserRoleRepository> _userRoles = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private ProvisionUserHandler CreateHandler()
    {
        return new ProvisionUserHandler(
            _users.Object,
            _roles.Object,
            _userRoles.Object,
            _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_NewUser_ShouldProvisionUser()
    {
        const string externalId = "iam-user-001";
        const string fullName = "Ahmed Ali";
        const string email = "ahmed@example.com";

        _users
            .Setup(x => x.GetByExternalIdAsync(
                externalId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        
        var publicRole = Role.Create(
            "public-user",
            "Public User",
            PortalType.Public,
            ScopeType.Self);

        _roles
            .Setup(x => x.GetByIdAsync(
                SystemRoleIds.PublicUser,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(publicRole);

        var handler = CreateHandler();

        var result = await handler.HandleAsync(
            new ProvisionUserCommand(
                externalId,
                fullName,
                email));

        Assert.NotEqual(Guid.Empty, result.UserId);
        Assert.Equal(externalId, result.ExternalId);
        Assert.Equal(fullName, result.FullName);
        Assert.Equal(email, result.Email);

        _users.Verify(
            x => x.AddAsync(
                It.Is<User>(u =>
                    u.ExternalId == externalId &&
                    u.FullName == fullName &&
                    u.Email == email),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ExistingExternalIdentity_ShouldThrowConflictException()
    {
        var existingUser = User.Create(
            "iam-user-001",
            "Ahmed Ali",
            "ahmed@example.com");

        _users
            .Setup(x => x.GetByExternalIdAsync(
                existingUser.ExternalId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.HandleAsync(
                new ProvisionUserCommand(
                    existingUser.ExternalId,
                    "Different Name",
                    "different@example.com")));

        _users.Verify(
            x => x.AddAsync(
                It.IsAny<User>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}