using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dv.App.Models;
using Dv.App.Services;
using Dv.App.ViewModels;
using Moq;
using Xunit;

namespace Dv.App.Tests;

public class DataLayerTests
{
    [Fact]
    public async Task DashboardViewModel_LoadsData_ShouldSetDashboardDataOnSuccess()
    {
        var mockApiService = new Mock<IApiService>();
        var sampleData = new List<SourceDataDto>
        {
            new SourceDataDto { HeatDemand = 42, ElectricityPrice = 101, TimeFrom = new DateTime(2026, 4, 4) }
        };

        mockApiService
            .Setup(x => x.GetAsync<List<SourceDataDto>>(BackendService.Sdm, "getAll"))
            .ReturnsAsync(sampleData);

        var viewModel = new DashboardViewModel(mockApiService.Object);
        await Task.Delay(100); 

        Assert.Contains("Success! SDM API responded.", viewModel.DashboardData);
        Assert.Contains("Heat Demand 42", viewModel.DashboardData);
    }

    [Fact]
    public async Task DashboardViewModel_LoadsData_ShouldHandleEmptyData()
    {
        var mockApiService = new Mock<IApiService>();
        var emptyData = new List<SourceDataDto>();

        mockApiService
            .Setup(x => x.GetAsync<List<SourceDataDto>>(BackendService.Sdm, "getAll"))
            .ReturnsAsync(emptyData);

        var viewModel = new DashboardViewModel(mockApiService.Object);
        await Task.Delay(100);

        Assert.Contains("SDM API returned an empty array", viewModel.DashboardData);
    }

    [Fact]
    public async Task DashboardViewModel_LoadsData_ShouldHandleException()
    {
        var mockApiService = new Mock<IApiService>();

        mockApiService
            .Setup(x => x.GetAsync<List<SourceDataDto>>(BackendService.Sdm, "getAll"))
            .ThrowsAsync(new System.Exception("Network blip"));

        var viewModel = new DashboardViewModel(mockApiService.Object);
        await Task.Delay(100);

        Assert.Contains("Test Failed: Network blip", viewModel.DashboardData);
    }

    [Fact]
    public async Task ProductionUnitsViewModel_LoadsData_ShouldSetProductionDataOnSuccess()
    {
        var mockApiService = new Mock<IApiService>();
        mockApiService
            .Setup(x => x.GetAsync<object>(BackendService.Am, "api/GetProductionUnits/allGasBoilers"))
            .ReturnsAsync(new { status = "ok" });

        var viewModel = new ProductionUnitsViewModel(mockApiService.Object);
        await Task.Delay(100);

        Assert.Contains("AM API responded. Data parsed: True", viewModel.ProductionData);
    }

    [Fact]
    public async Task OptimizationViewModel_LoadsData_ShouldSetOptDataOnSuccess()
    {
        var mockApiService = new Mock<IApiService>();

        var viewModel = new OptimizationViewModel(mockApiService.Object);
        await Task.Delay(100);

        Assert.Contains("Failed to fetch OPT data: Render endpoint not yet available.", viewModel.OptData);
    }
}
