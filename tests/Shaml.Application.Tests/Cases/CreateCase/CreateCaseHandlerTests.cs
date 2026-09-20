using Application.Cases.CreateCase;
using Application.Common.Interfaces;
using Domain.Cases;

namespace Shaml.Application.Tests.Cases.CreateCase;

public sealed class FakeUnitOfWork : IUnitOfWork
{
    public bool WasSaved { get; private set; }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        WasSaved = true;
        return Task.FromResult(1);
    }
}

public class CreateCaseHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldCreateAndSaveCase()
    {
        var repository = new FakeCaseRepository();

        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateCaseHandler(
            repository,
            unitOfWork);

        var command = new CreateCaseCommand(
            "SHAML-000001");

        var result = await handler.HandleAsync(command);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("SHAML-000001", result.CaseNumber);

        Assert.NotNull(repository.SavedCase);
        Assert.Equal(result.Id, repository.SavedCase.Id);
    }

    private sealed class FakeCaseRepository : ICaseRepository
    {
        public Case? SavedCase { get; private set; }

        public Task AddAsync(
            Case shamlCase,
            CancellationToken cancellationToken = default)
        {
            SavedCase = shamlCase;

            return Task.CompletedTask;
        }
    }
}