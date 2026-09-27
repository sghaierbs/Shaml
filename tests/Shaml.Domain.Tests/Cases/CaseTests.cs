using Domain.Cases;
using Domain.Cases.Events;

namespace Shaml.Domain.Tests.Cases;

public class CaseTests
{
    [Fact]
    public void WaitForSpecialist_NewCase_ShouldChangeStatus()
    {
        var @case = Case.Create("CASE-001",  Guid.NewGuid());

        @case.WaitForSpecialist();

        Assert.Equal(
            CaseStatus.WaitingForSpecialist,
            @case.Status);
    }

    [Fact]
    public void Start_WaitingForSpecialistCase_ShouldChangeStatusToInProgress()
    {
        var @case = Case.Create("CASE-001", Guid.NewGuid());

        @case.WaitForSpecialist();

        @case.Start();

        Assert.Equal(
            CaseStatus.InProgress,
            @case.Status);
    }
    
    [Fact]
    public void Create_ShouldCreateNewCase()
    {
        var shamlCase = Case.Create("SHAML-000001", Guid.NewGuid());

        Assert.NotEqual(Guid.Empty, shamlCase.Id);
        Assert.Equal("SHAML-000001", shamlCase.CaseNumber);
        Assert.Equal(CaseStatus.New, shamlCase.Status);

        Assert.Contains(
            shamlCase.DomainEvents,
            e => e is CaseCreatedEvent);
    }

    [Fact]
    public void Start_WhenCaseIsNew_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var @case = Case.Create("CASE-001", Guid.NewGuid());

        // Act
        var exception = Assert.Throws<InvalidOperationException>(
            () => @case.Start());

        // Assert
        Assert.Equal(
            "Cannot start a case with status New.",
            exception.Message);
    }
    
    [Fact]
    public void WaitForSpecialist_WhenCaseIsNew_ShouldChangeStatus()
    {
        var @case = Case.Create("CASE-001", Guid.NewGuid());

        @case.WaitForSpecialist();

        Assert.Equal(
            CaseStatus.WaitingForSpecialist,
            @case.Status);
    }

    [Fact]
    public void Complete_WhenCaseIsInProgress_ShouldCompleteCase()
    {
        // Arrange
        var @case = Case.Create("CASE-001", Guid.NewGuid());

        @case.WaitForSpecialist();
        @case.Start();

        // Act
        @case.Complete();

        // Assert
        Assert.Equal(CaseStatus.Completed, @case.Status);
    }

    [Fact]
    public void Complete_WhenCaseIsNew_ShouldThrowException()
    {
        var shamlCase = Case.Create("SHAML-000001", Guid.NewGuid());

        Assert.Throws<InvalidOperationException>(
            () => shamlCase.Complete());
    }
}