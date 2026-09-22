using Domain.Centers;

namespace Shaml.Domain.Tests.Centers;

public class CenterTests
{
    [Fact]
    public void Create_ShouldCreateActiveCenter()
    {
        var center = Center.Create(
            "RYD-001",
            "Riyadh Shaml Center",
            "Riyadh",
            "Riyadh");

        Assert.NotEqual(Guid.Empty, center.Id);
        Assert.Equal("RYD-001", center.Code);
        Assert.Equal("Riyadh Shaml Center", center.Name);
        Assert.Equal(CenterStatus.Active, center.Status);
    }

    [Fact]
    public void Create_WithoutName_ShouldThrowException()
    {
        Assert.Throws<ArgumentException>(() =>
            Center.Create(
                "RYD-001",
                "",
                "Riyadh",
                "Riyadh"));
    }

    [Fact]
    public void Deactivate_ShouldChangeStatusToInactive()
    {
        var center = Center.Create(
            "RYD-001",
            "Riyadh Shaml Center",
            "Riyadh",
            "Riyadh");

        center.Deactivate();

        Assert.Equal(CenterStatus.Inactive, center.Status);
    }

    [Fact]
    public void Activate_ShouldChangeStatusToActive()
    {
        var center = Center.Create(
            "RYD-001",
            "Riyadh Shaml Center",
            "Riyadh",
            "Riyadh");

        center.Deactivate();
        center.Activate();

        Assert.Equal(CenterStatus.Active, center.Status);
    }
}