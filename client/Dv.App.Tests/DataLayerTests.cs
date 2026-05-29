using Dv.App.Interfaces;
using Dv.App.Services;
using Dv.App.ViewModels;
using Moq;
using NUnit.Framework;

namespace Dv.App.Tests;

[TestFixture]
public class DataLayerTests
{
    [Test]
    public void DashboardViewModel_Initializes_Successfully()
    {
        var viewModel = new DashboardViewModel();

        Assert.That(viewModel, Is.Not.Null);
    }

    [Test]
    public void SettingsViewModel_Initialization_SetsDefaultValues()
    {
        var viewModel = new SettingsViewModel();

        Assert.That(viewModel, Is.Not.Null);
        Assert.That(viewModel.IsSystemTheme, Is.True);
    }

    [Test]
    public void ProductionUnitsViewModel_Initialization_SetsDefaultValues()
    {
        var viewModel = new ProductionUnitsViewModel();

        Assert.That(viewModel, Is.Not.Null);
        Assert.That(viewModel.MaintenanceSchedules, Is.Not.Null);
    }

    [Test]
    public void OptimizationViewModel_Initialization_WithValidDependencies()
    {
        var mockApiService = new Mock<IApiService>();
        var mockDialogService = new Mock<IDialogService>();
        var maintenanceService = new MaintenanceService(mockApiService.Object);

        var viewModel = new OptimizationViewModel(
            mockApiService.Object,
            maintenanceService,
            mockDialogService.Object);

        Assert.That(viewModel, Is.Not.Null);
        Assert.That(viewModel.Periods, Is.Not.Null);
        Assert.That(viewModel.Scenarios, Is.Not.Null);
    }

    [Test]
    public void OptimizationViewModel_Initialization_SetsDefaultSelectedPeriod()
    {
        var mockApiService = new Mock<IApiService>();
        var mockDialogService = new Mock<IDialogService>();
        var maintenanceService = new MaintenanceService(mockApiService.Object);

        var viewModel = new OptimizationViewModel(
            mockApiService.Object,
            maintenanceService,
            mockDialogService.Object);

        Assert.That(viewModel.SelectedPeriod, Is.EqualTo("Winter"));
    }

    [Test]
    public void OptimizationViewModel_Initialization_SetsDefaultSelectedScenario()
    {
        var mockApiService = new Mock<IApiService>();
        var mockDialogService = new Mock<IDialogService>();
        var maintenanceService = new MaintenanceService(mockApiService.Object);

        var viewModel = new OptimizationViewModel(
            mockApiService.Object,
            maintenanceService,
            mockDialogService.Object);

        Assert.That(viewModel.SelectedScenario, Is.EqualTo("Scenario 2"));
    }

    [Test]
    public void OptimizationViewModel_Initialization_CreatesFourBoilersForScenario1()
    {
        var mockApiService = new Mock<IApiService>();
        var mockDialogService = new Mock<IDialogService>();
        var maintenanceService = new MaintenanceService(mockApiService.Object);

        var viewModel = new OptimizationViewModel(
            mockApiService.Object,
            maintenanceService,
            mockDialogService.Object);

        viewModel.SelectedScenario = "Scenario 1";

        Assert.That(viewModel.CurrentContext.Boilers.Count, Is.EqualTo(4));
    }

    [Test]
    public void OptimizationViewModel_Initialization_CreatesFourBoilersForScenario2()
    {
        var mockApiService = new Mock<IApiService>();
        var mockDialogService = new Mock<IDialogService>();
        var maintenanceService = new MaintenanceService(mockApiService.Object);

        var viewModel = new OptimizationViewModel(
            mockApiService.Object,
            maintenanceService,
            mockDialogService.Object);

        viewModel.SelectedScenario = "Scenario 2";

        Assert.That(viewModel.CurrentContext.Boilers.Count, Is.EqualTo(4));
    }

    [Test]
    public void OptimizationViewModel_WinterPeriodDates_AreCorrect()
    {
        var mockApiService = new Mock<IApiService>();
        var mockDialogService = new Mock<IDialogService>();
        var maintenanceService = new MaintenanceService(mockApiService.Object);

        var viewModel = new OptimizationViewModel(
            mockApiService.Object,
            maintenanceService,
            mockDialogService.Object);

        viewModel.SelectedPeriod = "Winter";

        Assert.That(viewModel.CurrentContext.StartDate, Is.EqualTo(new DateTime(2026, 1, 5, 0, 0, 0)));
        Assert.That(viewModel.CurrentContext.EndDate, Is.EqualTo(new DateTime(2026, 1, 19, 0, 0, 0)));
    }

    [Test]
    public void OptimizationViewModel_SummerPeriodDates_AreCorrect()
    {
        var mockApiService = new Mock<IApiService>();
        var mockDialogService = new Mock<IDialogService>();
        var maintenanceService = new MaintenanceService(mockApiService.Object);

        var viewModel = new OptimizationViewModel(
            mockApiService.Object,
            maintenanceService,
            mockDialogService.Object);

        viewModel.SelectedPeriod = "Summer";

        Assert.That(viewModel.CurrentContext.StartDate, Is.EqualTo(new DateTime(2025, 9, 8, 0, 0, 0)));
        Assert.That(viewModel.CurrentContext.EndDate, Is.EqualTo(new DateTime(2025, 9, 21, 23, 59, 59)));
    }
}
