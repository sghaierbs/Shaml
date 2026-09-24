using Application.Centers.CreateCenter;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Centers;
using Moq;

namespace Shaml.Application.Tests.Centers.CreateCenter;

public class CreateCenterHandlerTests
{
    private readonly Mock<ICenterRepository> _centers = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private CreateCenterHandler CreateHandler()
    {
        return new CreateCenterHandler(
            _centers.Object,
            _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateCenter()
    {
        var command = new CreateCenterCommand(
            "RYD-001",
            "Riyadh Shaml Center",
            "Riyadh",
            "Riyadh",
            "King Fahd Road",
            "0111234567",
            "riyadh@shaml.sa");

        _centers
            .Setup(x => x.ExistsByCodeAsync(
                command.Code,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = CreateHandler();

        var result = await handler.HandleAsync(command);

        Assert.NotEqual(Guid.Empty, result.CenterId);
        Assert.Equal(command.Code, result.Code);
        Assert.Equal(command.Name, result.Name);
        Assert.Equal(command.Region, result.Region);
        Assert.Equal(command.City, result.City);

        _centers.Verify(
            x => x.AddAsync(
                It.Is<Center>(center =>
                    center.Code == command.Code &&
                    center.Name == command.Name &&
                    center.Region == command.Region &&
                    center.City == command.City &&
                    center.Address == command.Address &&
                    center.Phone == command.Phone &&
                    center.Email == command.Email),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateCode_ShouldThrowConflictException()
    {
        var command = new CreateCenterCommand(
            "RYD-001",
            "Riyadh Shaml Center",
            "Riyadh",
            "Riyadh");

        _centers
            .Setup(x => x.ExistsByCodeAsync(
                command.Code,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.HandleAsync(command));

        _centers.Verify(
            x => x.AddAsync(
                It.IsAny<Center>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_InvalidCenter_ShouldNotPersistAnything()
    {
        var command = new CreateCenterCommand(
            "",
            "Riyadh Shaml Center",
            "Riyadh",
            "Riyadh");

        _centers
            .Setup(x => x.ExistsByCodeAsync(
                command.Code,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = CreateHandler();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            handler.HandleAsync(command));

        _centers.Verify(
            x => x.AddAsync(
                It.IsAny<Center>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}