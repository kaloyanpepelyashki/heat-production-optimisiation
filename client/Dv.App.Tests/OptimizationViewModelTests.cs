using Xunit;
using Moq;
using Dv.App.ViewModels;
using Dv.App.Services;
using Microsoft.Extensions.Logging;
using Assert = Xunit.Assert;

namespace Dv.App.Tests.ViewModels;

public class OptimizationViewModelTests
{
    private readonly Mock<IApiService> _mockApiService;
    private readonly Mock<ILogger<OptimizationViewModel>> _mockLogger;
    private OptimizationViewModel _viewModel;

    public OptimizationViewModelTests()
    {
        _mockApiService = new Mock<IApiService>();
        _mockLogger = new Mock<ILogger<OptimizationViewModel>>();
    }

    [Fact]
    public void Constructor_InitializesFourMaintenancePeriodViewModels()
    {
        // Act
        _viewModel = new OptimizationViewModel(_mockApiService.Object, _mockLogger.Object);

        // Assert
        Assert.NotNull(_viewModel.SummerScenario1);
        Assert.NotNull(_viewModel.WinterScenario1);
        Assert.NotNull(_viewModel.SummerScenario2);
        Assert.NotNull(_viewModel.WinterScenario2);
    }

    [Fact]
    public void Constructor_SummerScenario1_HasCorrectDateRange()
    {
        // Act
        _viewModel = new OptimizationViewModel(_mockApiService.Object, _mockLogger.Object);

        // Assert
        Assert.Equal(new DateTime(2025, 9, 8, 0, 0, 0), _viewModel.SummerScenario1.PeriodStart);
        Assert.Equal(new DateTime(2025, 9, 21, 23, 59, 59), _viewModel.SummerScenario1.PeriodEnd);
    }

    [Fact]
    public void Constructor_WinterScenario1_HasCorrectDateRange()
    {
        // Act
        _viewModel = new OptimizationViewModel(_mockApiService.Object, _mockLogger.Object);

        // Assert
        Assert.Equal(new DateTime(2026, 1, 5, 0, 0, 0), _viewModel.WinterScenario1.PeriodStart);
        Assert.Equal(new DateTime(2026, 1, 18, 23, 59, 59), _viewModel.WinterScenario1.PeriodEnd);
    }

    [Fact]
    public void Constructor_Scenario1_HasFourBoilersForSummer()
    {
        // Act
        _viewModel = new OptimizationViewModel(_mockApiService.Object, _mockLogger.Object);

        // Assert
        Assert.Equal(4, _viewModel.SummerScenario1.Boilers.Count);
    }

    [Fact]
    public void Constructor_Scenario1_HasCorrectBoilerTypes()
    {
        // Act
        _viewModel = new OptimizationViewModel(_mockApiService.Object, _mockLogger.Object);

        // Assert
        Assert.Equal("GB1", _viewModel.SummerScenario1.Boilers[0].BoilerId);
        Assert.Equal("Gas", _viewModel.SummerScenario1.Boilers[0].FuelType);
        Assert.Equal("GB2", _viewModel.SummerScenario1.Boilers[1].BoilerId);
        Assert.Equal("OB1", _viewModel.SummerScenario1.Boilers[3].BoilerId);
        Assert.Equal("Oil", _viewModel.SummerScenario1.Boilers[3].FuelType);
    }

    [Fact]
    public void Constructor_Scenario2_HasDifferentBoilerConfiguration()
    {
        // Act
        _viewModel = new OptimizationViewModel(_mockApiService.Object, _mockLogger.Object);

        // Assert
        Assert.Equal(4, _viewModel.SummerScenario2.Boilers.Count);
        Assert.Equal("GM1", _viewModel.SummerScenario2.Boilers[2].BoilerId);
        Assert.Equal("Gas", _viewModel.SummerScenario2.Boilers[2].FuelType);
        Assert.Equal("EB1", _viewModel.SummerScenario2.Boilers[3].BoilerId);
        Assert.Equal("Electric", _viewModel.SummerScenario2.Boilers[3].FuelType);
    }

    [Fact]
    public void Constructor_AllScenarios_HavePeriodIdSet()
    {
        // Act
        _viewModel = new OptimizationViewModel(_mockApiService.Object, _mockLogger.Object);

        // Assert
        Assert.Equal("1", _viewModel.SummerScenario1.Scenario);
        Assert.Equal("1", _viewModel.WinterScenario1.Scenario);
        Assert.Equal("2", _viewModel.SummerScenario2.Scenario);
        Assert.Equal("2", _viewModel.WinterScenario2.Scenario);
    }

    [Fact]
    public void Constructor_AllScenarios_HavePeriodLabels()
    {
        // Act
        _viewModel = new OptimizationViewModel(_mockApiService.Object, _mockLogger.Object);

        // Assert
        Assert.Equal("Summer", _viewModel.SummerScenario1.PeriodName);
        Assert.Equal("Winter", _viewModel.WinterScenario1.PeriodName);
        Assert.Equal("Summer", _viewModel.SummerScenario2.PeriodName);
        Assert.Equal("Winter", _viewModel.WinterScenario2.PeriodName);
    }
}
