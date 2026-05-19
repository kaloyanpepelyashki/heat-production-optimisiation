using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dv.App.Models;
using Dv.App.Services;
using Dv.App.ViewModels;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Dv.App.Tests;

public class DataLayerTests
{
    [Fact]
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

        Assert.Contains("Success! SDM API responded.", viewModel.DashboardData);
        Assert.Contains("Heat Demand 42", viewModel.DashboardData);
    }

    [Fact]
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

        Assert.Contains("SDM API returned an empty array", viewModel.DashboardData);
    }

    [Fact]
    public async Task DashboardViewModel_LoadsData_ShouldHandleException()
    {
        var mockApiService = new Mock<IApiService>();
        var mockLogger = new Mock<ILogger<DashboardViewModel>>();

        mockApiService
            .Setup(x => x.GetAsync<List<SourceDataDto>>(BackendService.Sdm, "getAll"))
            .ThrowsAsync(new System.Exception("Network blip"));

        var viewModel = new DashboardViewModel(mockApiService.Object, mockLogger.Object);
        await viewModel.InitializationTask;

        Assert.Contains("Test Failed: Network blip", viewModel.DashboardData);
    }

    [Fact]
    public async Task ProductionUnitsViewModel_LoadsData_ShouldSetProductionDataOnSuccess()
    {
        var mockApiService = new Mock<IApiService>();
        var mockLogger = new Mock<ILogger<ProductionUnitsViewModel>>();
        mockApiService
            .Setup(x => x.GetAsync<object>(BackendService.Am, "api/GetProductionUnits/allGasBoilers"))
            .ReturnsAsync(new { status = "ok" });

        var viewModel = new ProductionUnitsViewModel(mockApiService.Object, mockLogger.Object);
        await viewModel.InitializationTask;

        Assert.Contains("AM API responded. Data parsed: True", viewModel.ProductionData);
    }

    [Fact]
    public void SettingsViewModel_Initialization_SetsDefaultValues()
    {
        var viewModel = new SettingsViewModel();

        Assert.NotNull(viewModel);
    }

    [Fact]
    public void OptimizationViewModel_Initialization_SetsScenarios_Positive()
    {
        var mockApiService = new Mock<IApiService>();
        var mockLogger = new Mock<ILogger<OptimizationViewModel>>();

        var viewModel = new OptimizationViewModel(mockApiService.Object, mockLogger.Object);

        Assert.NotNull(viewModel.SummerScenario1);
        Assert.NotNull(viewModel.WinterScenario1);
        Assert.NotNull(viewModel.SummerScenario2);
        Assert.NotNull(viewModel.WinterScenario2);
        Assert.Equal("Summer", viewModel.SummerScenario1?.PeriodName);
        Assert.Equal("Winter", viewModel.WinterScenario1?.PeriodName);
    }

    [Fact]
    public void OptimizationViewModel_Scenario_Instantiates_Boilers_Edge()
    {
        var mockApiService = new Mock<IApiService>();
        var mockLogger = new Mock<ILogger<OptimizationViewModel>>();

        var viewModel = new OptimizationViewModel(mockApiService.Object, mockLogger.Object);

        Assert.Equal(4, viewModel.SummerScenario1?.Boilers.Count ?? 0);
        Assert.Equal(4, viewModel.SummerScenario2?.Boilers.Count ?? 0);
    }
}
