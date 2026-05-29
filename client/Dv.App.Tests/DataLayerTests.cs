using Dv.App.Models;
using Dv.App.Services;
using Dv.App.ViewModels;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Dv.App.Tests;

[TestFixture]
public class DataLayerTests
{
    [Test]
    public async Task DashboardViewModel_LoadsData_ShouldSetDashboardDataOnSuccess()
    {
        var mockApiService = new Mock<IApiService>();
        var mockLogger = new Mock<ILogger<DashboardViewModel>>();
        var sampleData = new List<SourceDataDto>
        {
            new SourceDataDto { HeatDemand = 42, ElectricityPrice = 101, TimeFrom = new DateTime(2026, 4, 4) }
        };

        mockApiService
            .Setup(x => x.GetAsync<List<SourceDataDto>>(BackendService.Sdm, "getAll"))
            .ReturnsAsync(sampleData);

        var viewModel = new DashboardViewModel(mockApiService.Object, mockLogger.Object);
        await viewModel.InitializationTask;

        Assert.That(viewModel.DashboardData, Does.Contain("Success! SDM API responded."));
        Assert.That(viewModel.DashboardData, Does.Contain("Heat Demand 42"));
    }

    [Test]
    public async Task DashboardViewModel_LoadsData_ShouldHandleEmptyData()
    {
        var mockApiService = new Mock<IApiService>();
        var mockLogger = new Mock<ILogger<DashboardViewModel>>();
        var emptyData = new List<SourceDataDto>();

        mockApiService
            .Setup(x => x.GetAsync<List<SourceDataDto>>(BackendService.Sdm, "getAll"))
            .ReturnsAsync(emptyData);

        var viewModel = new DashboardViewModel(mockApiService.Object, mockLogger.Object);
        await viewModel.InitializationTask;

        Assert.That(viewModel.DashboardData, Does.Contain("SDM API returned an empty array"));
    }

    [Test]
    public async Task DashboardViewModel_LoadsData_ShouldHandleException()
    {
        var mockApiService = new Mock<IApiService>();
        var mockLogger = new Mock<ILogger<DashboardViewModel>>();

        mockApiService
            .Setup(x => x.GetAsync<List<SourceDataDto>>(BackendService.Sdm, "getAll"))
            .ThrowsAsync(new Exception("Network blip"));

        var viewModel = new DashboardViewModel(mockApiService.Object, mockLogger.Object);
        await viewModel.InitializationTask;

        Assert.That(viewModel.DashboardData, Does.Contain("Test Failed: Network blip"));
    }

    [Test]
    public async Task ProductionUnitsViewModel_LoadsData_ShouldSetProductionDataOnSuccess()
    {
        var mockApiService = new Mock<IApiService>();
        var mockLogger = new Mock<ILogger<ProductionUnitsViewModel>>();
        mockApiService
            .Setup(x => x.GetAsync<object>(BackendService.Am, "api/GetProductionUnits/allGasBoilers"))
            .ReturnsAsync(new { status = "ok" });

        var viewModel = new ProductionUnitsViewModel(mockApiService.Object, mockLogger.Object);
        await viewModel.InitializationTask;

        Assert.That(viewModel.ProductionData, Does.Contain("AM API responded. Data parsed: True"));
    }

    [Test]
    public void SettingsViewModel_Initialization_SetsDefaultValues()
    {
        var viewModel = new SettingsViewModel();

        Assert.That(viewModel, Is.Not.Null);
    }

    [Test]
    public void OptimizationViewModel_Initialization_SetsScenarios_Positive()
    {
        var mockApiService = new Mock<IApiService>();
        var mockLogger = new Mock<ILogger<OptimizationViewModel>>();

        var viewModel = new OptimizationViewModel(mockApiService.Object, mockLogger.Object);

        Assert.That(viewModel.SummerScenario1, Is.Not.Null);
        Assert.That(viewModel.WinterScenario1, Is.Not.Null);
        Assert.That(viewModel.SummerScenario2, Is.Not.Null);
        Assert.That(viewModel.WinterScenario2, Is.Not.Null);
        Assert.That(viewModel.SummerScenario1.PeriodName, Is.EqualTo("Summer"));
        Assert.That(viewModel.WinterScenario1.PeriodName, Is.EqualTo("Winter"));
    }

    [Test]
    public void OptimizationViewModel_Scenario_Instantiates_Boilers_Edge()
    {
        var mockApiService = new Mock<IApiService>();
        var mockLogger = new Mock<ILogger<OptimizationViewModel>>();

        var viewModel = new OptimizationViewModel(mockApiService.Object, mockLogger.Object);

        Assert.That(viewModel.SummerScenario1?.Boilers.Count ?? 0, Is.EqualTo(4));
        Assert.That(viewModel.SummerScenario2?.Boilers.Count ?? 0, Is.EqualTo(4));
    }
}
