using Dv.App.Interfaces;
using Dv.App.Services;
using Dv.App.ViewModels;
using Moq;
using NUnit.Framework;

namespace Dv.App.Tests;

[TestFixture]
public class OptimizationViewModelTests
{
    private Mock<IApiService> _mockApiService = null!;
    private Mock<IDialogService> _mockDialogService = null!;
    private OptimizationViewModel _viewModel = null!;

    [SetUp]
    public void SetUp()
    {
        _mockApiService = new Mock<IApiService>();
        _mockDialogService = new Mock<IDialogService>();
        var maintenanceService = new MaintenanceService(_mockApiService.Object);
        _viewModel = new OptimizationViewModel(
            _mockApiService.Object,
            maintenanceService,
            _mockDialogService.Object);
    }

    [Test]
    public void Constructor_InitializesRequiredComponents()
    {
        Assert.That(_viewModel, Is.Not.Null);
        Assert.That(_viewModel.Maintenance, Is.Not.Null);
        Assert.That(_viewModel.ChartsVM, Is.Not.Null);
    }

    [Test]
    public void Constructor_SetsDefaultPeriodToWinter()
    {
        Assert.That(_viewModel.SelectedPeriod, Is.EqualTo("Winter"));
    }

    [Test]
    public void Constructor_SetsDefaultScenarioToScenario2()
    {
        Assert.That(_viewModel.SelectedScenario, Is.EqualTo("Scenario 2"));
    }

    [Test]
    public void Constructor_InitializesCurrentContextWithDefaults()
    {
        Assert.That(_viewModel.CurrentContext, Is.Not.Null);
        Assert.That(_viewModel.CurrentContext.Period, Is.EqualTo("Winter"));
        Assert.That(_viewModel.CurrentContext.Scenario, Is.EqualTo("Scenario 2"));
    }

    [Test]
    public void Periods_ContainsSummerAndWinter()
    {
        Assert.That(_viewModel.Periods.Count, Is.EqualTo(2));
        Assert.That(_viewModel.Periods, Does.Contain("Summer"));
        Assert.That(_viewModel.Periods, Does.Contain("Winter"));
    }

    [Test]
    public void Scenarios_ContainsScenario1And2()
    {
        Assert.That(_viewModel.Scenarios.Count, Is.EqualTo(2));
        Assert.That(_viewModel.Scenarios, Does.Contain("Scenario 1"));
        Assert.That(_viewModel.Scenarios, Does.Contain("Scenario 2"));
    }

    [Test]
    public void CurrentContext_WinterScenario1_HasCorrectDateRange()
    {
        _viewModel.SelectedPeriod = "Winter";
        _viewModel.SelectedScenario = "Scenario 1";

        Assert.That(_viewModel.CurrentContext.StartDate, Is.EqualTo(new DateTime(2026, 1, 5, 0, 0, 0)));
        Assert.That(_viewModel.CurrentContext.EndDate, Is.EqualTo(new DateTime(2026, 1, 19, 0, 0, 0)));
    }

    [Test]
    public void CurrentContext_SummerScenario1_HasCorrectDateRange()
    {
        _viewModel.SelectedPeriod = "Summer";
        _viewModel.SelectedScenario = "Scenario 1";

        Assert.That(_viewModel.CurrentContext.StartDate, Is.EqualTo(new DateTime(2025, 9, 8, 0, 0, 0)));
        Assert.That(_viewModel.CurrentContext.EndDate, Is.EqualTo(new DateTime(2025, 9, 21, 23, 59, 59)));
    }

    [Test]
    public void CurrentContext_Scenario1_HasFourBoilersWithCorrectIds()
    {
        _viewModel.SelectedScenario = "Scenario 1";

        Assert.That(_viewModel.CurrentContext.Boilers.Count, Is.EqualTo(4));
        Assert.That(_viewModel.CurrentContext.Boilers[0].BoilerId, Is.EqualTo("GB1"));
        Assert.That(_viewModel.CurrentContext.Boilers[1].BoilerId, Is.EqualTo("GB2"));
        Assert.That(_viewModel.CurrentContext.Boilers[2].BoilerId, Is.EqualTo("GB3"));
        Assert.That(_viewModel.CurrentContext.Boilers[3].BoilerId, Is.EqualTo("OB1"));
    }

    [Test]
    public void CurrentContext_Scenario1_HasCorrectBoilerFuelTypes()
    {
        _viewModel.SelectedScenario = "Scenario 1";

        var boilers = _viewModel.CurrentContext.Boilers;
        Assert.That(boilers[0].FuelType, Is.EqualTo("Gas"));
        Assert.That(boilers[1].FuelType, Is.EqualTo("Gas"));
        Assert.That(boilers[2].FuelType, Is.EqualTo("Gas"));
        Assert.That(boilers[3].FuelType, Is.EqualTo("Oil"));
    }

    [Test]
    public void CurrentContext_Scenario2_HasDifferentBoilerConfiguration()
    {
        _viewModel.SelectedScenario = "Scenario 2";

        Assert.That(_viewModel.CurrentContext.Boilers.Count, Is.EqualTo(4));
        Assert.That(_viewModel.CurrentContext.Boilers[0].BoilerId, Is.EqualTo("GB1"));
        Assert.That(_viewModel.CurrentContext.Boilers[1].BoilerId, Is.EqualTo("GB3"));
        Assert.That(_viewModel.CurrentContext.Boilers[2].BoilerId, Is.EqualTo("GM1"));
        Assert.That(_viewModel.CurrentContext.Boilers[3].BoilerId, Is.EqualTo("EB1"));
    }

    [Test]
    public void CurrentContext_Scenario2_HasCorrectBoilerFuelTypes()
    {
        _viewModel.SelectedScenario = "Scenario 2";

        var boilers = _viewModel.CurrentContext.Boilers;
        Assert.That(boilers[0].FuelType, Is.EqualTo("Gas"));
        Assert.That(boilers[1].FuelType, Is.EqualTo("Gas"));
        Assert.That(boilers[2].FuelType, Is.EqualTo("Gas"));
        Assert.That(boilers[3].FuelType, Is.EqualTo("Electric"));
    }

    [Test]
    public void SelectedPeriodChange_UpdatesCurrentContext()
    {
        _viewModel.SelectedPeriod = "Summer";

        Assert.That(_viewModel.CurrentContext.Period, Is.EqualTo("Summer"));
        Assert.That(_viewModel.CurrentContext.PeriodId, Is.EqualTo(1));
    }

    [Test]
    public void SelectedScenarioChange_UpdatesCurrentContext()
    {
        _viewModel.SelectedScenario = "Scenario 1";

        Assert.That(_viewModel.CurrentContext.Scenario, Is.EqualTo("Scenario 1"));
        Assert.That(_viewModel.CurrentContext.ScenarioId, Is.EqualTo(1));
    }

    [Test]
    public void IsLoading_DefaultsToFalse()
    {
        Assert.That(_viewModel.IsLoading, Is.False);
    }

    [Test]
    public void HasOptimizationResults_DefaultsToFalse()
    {
        Assert.That(_viewModel.HasOptimizationResults, Is.False);
    }

    [Test]
    public void ErrorMessage_DefaultsToNull()
    {
        Assert.That(_viewModel.ErrorMessage, Is.Null);
    }
}
