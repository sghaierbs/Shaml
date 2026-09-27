using Application.Cases.RouteToSpecialist;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Assignments;
using Domain.Cases;
using Shaml.Application.Tests.Cases.CreateCase;

namespace Shaml.Application.Tests.Cases.RouteToSpecialist;

public sealed class RouteCaseToSpecialistHandlerTests
{
    
    [Fact]
    public async Task HandleAsync_WhenCaseDoesNotExist_ShouldThrowNotFoundException()
    {
        var caseRepository = new FakeCaseRepository(
            Case.Create("OTHER-CASE",  Guid.NewGuid()));

        var assignmentRepository = new FakeAssignmentRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new RouteCaseToSpecialistHandler(
            caseRepository,
            assignmentRepository,
            unitOfWork);

        var command = new RouteCaseToSpecialistCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid());

        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.HandleAsync(command));

        Assert.Null(assignmentRepository.SavedAssignment);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }
    
    [Fact]
    public async Task HandleAsync_ShouldRouteCaseAndCreateRoleQueueAssignment()
    {
        // Arrange
        var shamlCase = Case.Create("SHAML-000001", Guid.NewGuid());

        var caseRepository = new FakeCaseRepository(shamlCase);
        var assignmentRepository = new FakeAssignmentRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new RouteCaseToSpecialistHandler(
            caseRepository,
            assignmentRepository,
            unitOfWork);

        var specialistRoleId = Guid.NewGuid();
        var centerId = Guid.NewGuid();

        var command = new RouteCaseToSpecialistCommand(
            shamlCase.Id,
            specialistRoleId,
            centerId);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.Equal(
            CaseStatus.WaitingForSpecialist,
            shamlCase.Status);

        Assert.Equal(
            CaseStatus.WaitingForSpecialist,
            result.CaseStatus);

        Assert.NotNull(assignmentRepository.SavedAssignment);

        var assignment = assignmentRepository.SavedAssignment;

        Assert.Equal(shamlCase.Id, assignment.CaseId);
        Assert.Equal(AssignmentTaskCodes.ReviewCase, assignment.TaskCode);
        Assert.Equal(AssignmentTargetType.RoleQueue, assignment.TargetType);
        Assert.Equal(specialistRoleId, assignment.TargetRoleId);
        Assert.Equal(centerId, assignment.TargetCenterId);
        Assert.Equal(AssignmentStatus.Open, assignment.Status);

        Assert.Equal(assignment.Id, result.AssignmentId);

        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    private sealed class FakeCaseRepository : ICaseRepository
    {
        private readonly Case _case;

        public FakeCaseRepository(Case shamlCase)
        {
            _case = shamlCase;
        }

        public Task<Case?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Case?>(
                _case.Id == id ? _case : null);
        }

        public Task AddAsync(
            Case shamlCase,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
    
    
    private sealed class FakeAssignmentRepository : IAssignmentRepository
    {
        public Assignment? SavedAssignment { get; private set; }

        public Task AddAsync(
            Assignment assignment,
            CancellationToken cancellationToken = default)
        {
            SavedAssignment = assignment;

            return Task.CompletedTask;
        }

        public Task<Assignment?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Assignment?>(null);
        }

        public Task<IReadOnlyCollection<Assignment>> GetOpenRoleQueueAsync(
            Guid roleId,
            Guid centerId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<Assignment>>(
                Array.Empty<Assignment>());
        }

        public Task<IReadOnlyCollection<Assignment>> GetWorkForUserRoleAsync(
            Guid userRoleId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<Assignment>>(
                Array.Empty<Assignment>());
        }

        public Task<IReadOnlyCollection<Assignment>> GetWorkForUserAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<Assignment>>(
                Array.Empty<Assignment>());
        }
    }
    
    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCallCount { get; private set; }

        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            SaveChangesCallCount++;

            return Task.FromResult(1);
        }
    }
}