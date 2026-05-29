using Dv.App.Services;
using Dv.App.ViewModels;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Dv.App.Tests;

[TestFixture]
public class OptimizationViewModelTests
{
    private Mock<IApiService> _mockApiService = null!;
    private Mock<ILogger<OptimizationViewModel>> _mockLogger = null!;
    private OptimizationViewModel _viewModel = null!;

    [SetUp]
    public void SetUp()
    {
        _mockApiService = new Mock<IApiService>();
        _mockLogger = new Mock<ILogger<OptimizationViewModel>>();
        _viewModel = new OptimizationViewModel(_mockApiService.Object, _mockLogger.Object);
    }

    [Test]
    public void Constructor_InitializesFourMaintenancePeriodViewModels()
    {
        Assert.That(_viewModel.SummerScenario1, Is.Not.Null);
        Assert.That(_viewModel.WinterScenario1, Is.Not.Null);
        Assert.That(_viewModel.SummerScenario2, Is.Not.Null);
        Assert.That(_viewModel.WinterScenario2, Is.Not.Null);
    }

    [Test]
    public void Constructor_SummerScenario1_HasCorrectDateRange()
    {
        Assert.That(_viewModel.SummerScenario1.PeriodStart, Is.EqualTo(new DateTime(2025, 9, 8, 0, 0, 0)));
        Assert.That(_viewModel.SummerScenario1.PeriodEnd, Is.EqualTo(new DateTime(2025, 9, 21, 23, 59, 59)));
    }

    [Test]
    public void Constructor_WinterScenario1_HasCorrectDateRange()
    {
        Assert.That(_viewModel.WinterScenario1.PeriodStart, Is.EqualTo(new DateTime(2026, 1, 5, 0, 0, 0)));
        Assert.That(_viewModel.WinterScenario1.PeriodEnd, Is.EqualTo(new DateTime(2026, 1, 18, 23, 59, 59)));
    }

    [Test]
    public void Constructor_Scenario1_HasFourBoilersForSummer()
    {
        Assert.That(_viewModel.SummerScenario1.Boilers.Count, Is.EqualTo(4));
    }

    [Test]
    public void Constructor_Scenario1_HasCorrectBoilerTypes()
    {
        Assert.That(_viewModel.SummerScenario1.Boilers[0].BoilerId, Is.EqualTo("GB1"));
        Assert.That(_viewModel.SummerScenario1.Boilers[0].FuelType, Is.EqualTo("Gas"));
        Assert.That(_viewModel.SummerScenario1.Boilers[1].BoilerId, Is.EqualTo("GB2"));
        Assert.That(_viewModel.SummerScenario1.Boilers[3].BoilerId, Is.EqualTo("OB1"));
        Assert.That(_viewModel.SummerScenario1.Boilers[3].FuelType, Is.EqualTo("Oil"));
    }

    [Test]
    public void Constructor_Scenario2_HasDifferentBoilerConfiguration()
    {
        Assert.That(_viewModel.SummerScenario2.Boilers.Count, Is.EqualTo(4));
        Assert.That(_viewModel.SummerScenario2.Boilers[2].BoilerId, Is.EqualTo("GM1"));
        Assert.That(_viewModel.SummerScenario2.Boilers[2].FuelType, Is.EqualTo("Gas"));
        Assert.That(_viewModel.SummerScenario2.Boilers[3].BoilerId, Is.EqualTo("EB1"));
        Assert.That(_viewModel.SummerScenario2.Boilers[3].FuelType, Is.EqualTo("Electric"));
    }

    [Test]
    public void Constructor_AllScenarios_HavePeriodIdSet()
    {
        Assert.That(_viewModel.SummerScenario1.Scenario, Is.EqualTo("1"));
        Assert.That(_viewModel.WinterScenario1.Scenario, Is.EqualTo("1"));
        Assert.That(_viewModel.SummerScenario2.Scenario, Is.EqualTo("2"));
        Assert.That(_viewModel.WinterScenario2.Scenario, Is.EqualTo("2"));
    }

    [Test]
    public void Constructor_AllScenarios_HavePeriodLabels()
    {
        Assert.That(_viewModel.SummerScenario1.PeriodName, Is.EqualTo("Summer"));
        Assert.That(_viewModel.WinterScenario1.PeriodName, Is.EqualTo("Winter"));
        Assert.That(_viewModel.SummerScenario2.PeriodName, Is.EqualTo("Summer"));
        Assert.That(_viewModel.WinterScenario2.PeriodName, Is.EqualTo("Winter"));
    }
}
