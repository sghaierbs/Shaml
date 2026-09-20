using Domain.Cases;
using Shaml.Domain.Cases;
using Domain.Cases.Events;
using Shaml.Domain.Cases.Events;

namespace Shaml.Domain.Tests.Cases;

public class CaseTests
{
    [Fact]
    public void Create_ShouldCreateNewCase()
    {
        var shamlCase = Case.Create("SHAML-000001");

        Assert.NotEqual(Guid.Empty, shamlCase.Id);
        Assert.Equal("SHAML-000001", shamlCase.CaseNumber);
        Assert.Equal(CaseStatus.New, shamlCase.Status);

        Assert.Contains(
            shamlCase.DomainEvents,
            e => e is CaseCreatedEvent);
    }

    [Fact]
    public void Start_WhenCaseIsNew_ShouldChangeStatusToInProgress()
    {
        var shamlCase = Case.Create("SHAML-000001");

        shamlCase.Start();

        Assert.Equal(
            CaseStatus.InProgress,
            shamlCase.Status);

        Assert.Contains(
            shamlCase.DomainEvents,
            e => e is CaseStartedEvent);
    }

    [Fact]
    public void Complete_WhenCaseIsInProgress_ShouldCompleteCase()
    {
        var shamlCase = Case.Create("SHAML-000001");

        shamlCase.Start();
        shamlCase.Complete();

        Assert.Equal(
            CaseStatus.Completed,
            shamlCase.Status);

        Assert.Contains(
            shamlCase.DomainEvents,
            e => e is CaseCompletedEvent);
    }

    [Fact]
    public void Complete_WhenCaseIsNew_ShouldThrowException()
    {
        var shamlCase = Case.Create("SHAML-000001");

        Assert.Throws<InvalidOperationException>(
            () => shamlCase.Complete());
    }
}