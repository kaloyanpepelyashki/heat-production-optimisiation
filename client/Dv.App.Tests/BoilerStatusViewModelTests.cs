using Xunit;
using Dv.App.ViewModels;
using Assert = Xunit.Assert;

namespace Dv.App.Tests.ViewModels;

public class BoilerStatusViewModelTests
{
    [Fact]
    public void Constructor_InitializesProperties()
    {
        // Act
        var boiler = new BoilerStatusViewModel("GB1", "Gas", "Summer");

        // Assert
        Assert.Equal("GB1", boiler.BoilerId);
        Assert.Equal("Gas", boiler.FuelType);
    }

    [Xunit.Theory]
    [InlineData("GB1", "Gas")]
    [InlineData("OB1", "Oil")]
    [InlineData("GM1", "Gas")]
    [InlineData("EB1", "Electric")]
    public void Constructor_AcceptsVariousBoilerTypes(string id, string fuelType)
    {
        // Act
        var boiler = new BoilerStatusViewModel(id, fuelType, "Winter");

        // Assert
        Assert.Equal(id, boiler.BoilerId);
        Assert.Equal(fuelType, boiler.FuelType);
    }

    [Fact]
    public void StatusText_ReturnsCorrectString()
    {
        // Arrange
        var boiler = new BoilerStatusViewModel("GB1", "Gas", "Summer");

        // Assert
        Assert.NotNull(boiler.StatusText);
        Assert.NotEmpty(boiler.StatusText);
    }
}