using Moq;
using Opt.Api.Application.Interfaces;
using Opt.Api.Application.Services;
using Opt.Api.Domain.Models;
using Opt.Api.DTOs;
using Microsoft.Extensions.Logging.Abstractions;

namespace Opt.Api.Tests;

public class Optimizer_Test
{
    Mock<IAssetDataProvider> assetProvider = new Mock<IAssetDataProvider>();
    Mock<ISourceDataProvider> sourceProvider = new Mock<ISourceDataProvider>();

    [Fact]
    public async Task BuildHourlyResult_GB_HasConstantCostPerHeat()
    {
        var bundle = new AssetDataBundle
        {
            GasBoilers = new[]
            {
                new GasBoiler { Id = 1, Name = "GB1", MaxHeat = 5.0f, ProductionCost = 500f }
            }
        };

        assetProvider.Setup(x => x.GetAssetDataAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(bundle);

        sourceProvider.Setup(x => x.GetSourceDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SourceDataPoint>
            {
                new SourceDataPoint { PeriodId = 1, TimeFrom = new DateTime(2025, 1, 1, 10, 0, 0), TimeTo = new DateTime(2025, 1, 1, 11, 0, 0), HeatDemand = 5.0, ElectricityPrice = 100 },
                new SourceDataPoint { PeriodId = 1, TimeFrom = new DateTime(2025, 1, 1, 11, 0, 0), TimeTo = new DateTime(2025, 1, 1, 12, 0, 0), HeatDemand = 5.0, ElectricityPrice = 500 }
            });

        var optimizer = new Optimizer(assetProvider.Object, sourceProvider.Object, NullLogger<Optimizer>.Instance);
        var request = new OptimizationRequestDto { ScenarioId = 1, PeriodId = 1, TimeFrom = new DateTime(2025, 1, 1), TimeTo = new DateTime(2025, 1, 2) };

        var result = await optimizer.OptimizeAsync(request, CancellationToken.None);

        Assert.Equal(2, result.OptResultsHourly.Count);
        Assert.Equal(result.OptResultsHourly[0].Expenses, result.OptResultsHourly[1].Expenses);
    }

    [Fact]
    public async Task BuildHourlyResult_GM_CostPerHeatDecreases_WhenElectricityPriceIncreases()
    {
        var bundle = new AssetDataBundle
        {
            GasMotors = new[]
            {
                new GasMotor { Id = 1, Name = "GM1", MaxHeat = 10.0f, MaxElectricity = 4.0f, ProductionCost = 600f }
            }
        };

        assetProvider.Setup(x => x.GetAssetDataAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(bundle);

        sourceProvider.Setup(x => x.GetSourceDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SourceDataPoint>
            {
                new SourceDataPoint { PeriodId = 2, TimeFrom = new DateTime(2026, 1, 1, 10, 0, 0), TimeTo = new DateTime(2026, 1, 1, 11, 0, 0), HeatDemand = 10.0, ElectricityPrice = -50 },
                new SourceDataPoint { PeriodId = 2, TimeFrom = new DateTime(2026, 1, 1, 11, 0, 0), TimeTo = new DateTime(2026, 1, 1, 12, 0, 0), HeatDemand = 10.0, ElectricityPrice = 100 }
            });

        var optimizer = new Optimizer(assetProvider.Object, sourceProvider.Object, NullLogger<Optimizer>.Instance);
        var request = new OptimizationRequestDto { ScenarioId = 2, PeriodId = 2, TimeFrom = new DateTime(2026, 1, 1), TimeTo = new DateTime(2026, 1, 2) };

        var result = await optimizer.OptimizeAsync(request, CancellationToken.None);

        var negativePriceCost = result.OptResultsHourly[0].Expenses;
        var positivePriceCost = result.OptResultsHourly[1].Expenses;

        Assert.True(positivePriceCost < negativePriceCost);
    }

    [Fact]
    public async Task BuildHourlyResult_EB_CostPerHeatIncreases_WhenElectricityPriceIncreases()
    {
        var bundle = new AssetDataBundle
        {
            ElectricBoilers = new[]
            {
                new ElectricBoiler { Id = 1, Name = "EB1", MaxHeat = 8.0f, MaxElectricity = -5.0f, ProductionCost = 100f }
            }
        };

        assetProvider.Setup(x => x.GetAssetDataAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(bundle);

        sourceProvider.Setup(x => x.GetSourceDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SourceDataPoint>
            {
                new SourceDataPoint { PeriodId = 2, TimeFrom = new DateTime(2026, 1, 1, 10, 0, 0), TimeTo = new DateTime(2026, 1, 1, 11, 0, 0), HeatDemand = 8.0, ElectricityPrice = -50 },
                new SourceDataPoint { PeriodId = 2, TimeFrom = new DateTime(2026, 1, 1, 11, 0, 0), TimeTo = new DateTime(2026, 1, 1, 12, 0, 0), HeatDemand = 8.0, ElectricityPrice = 100 }
            });

        var optimizer = new Optimizer(assetProvider.Object, sourceProvider.Object, NullLogger<Optimizer>.Instance);
        var request = new OptimizationRequestDto { ScenarioId = 2, PeriodId = 2, TimeFrom = new DateTime(2026, 1, 1), TimeTo = new DateTime(2026, 1, 2) };

        var result = await optimizer.OptimizeAsync(request, CancellationToken.None);

        var negativePriceCost = result.OptResultsHourly[0].Expenses;
        var positivePriceCost = result.OptResultsHourly[1].Expenses;

        Assert.True(positivePriceCost > negativePriceCost);
    }

    [Fact]
    public async Task BuildHourlyResult_Ensure_GM_And_EB_AreNeverDispatchedInSameHour()
    {
        var bundle = new AssetDataBundle
        {
            GasBoilers = new[] { new GasBoiler { Id = 1, Name = "GB1", MaxHeat = 20.0f, ProductionCost = 400f } },
            ElectricBoilers = new[] { new ElectricBoiler { Id = 1, Name = "EB1", MaxHeat = 10.0f, MaxElectricity = -5.0f, ProductionCost = 100f } },
            GasMotors = new[] { new GasMotor { Id = 1, Name = "GM1", MaxHeat = 10.0f, MaxElectricity = 4.0f, ProductionCost = 600f } }
        };

        assetProvider.Setup(x => x.GetAssetDataAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(bundle);

        sourceProvider.Setup(x => x.GetSourceDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SourceDataPoint>
            {
                new SourceDataPoint { PeriodId = 2, TimeFrom = new DateTime(2026, 1, 1, 10, 0, 0), TimeTo = new DateTime(2026, 1, 1, 11, 0, 0), HeatDemand = 15.0, ElectricityPrice = 500 }
            });

        var optimizer = new Optimizer(assetProvider.Object, sourceProvider.Object, NullLogger<Optimizer>.Instance);
        var request = new OptimizationRequestDto { ScenarioId = 2, PeriodId = 2, TimeFrom = new DateTime(2026, 1, 1), TimeTo = new DateTime(2026, 1, 2) };

        var result = await optimizer.OptimizeAsync(request, CancellationToken.None);

        var dispatchedUnits = result.OptResultsHourly[0].Units;
        var hasEb = dispatchedUnits.Any(u => u.UnitType == "EB");
        var hasGm = dispatchedUnits.Any(u => u.UnitType == "GM");

        Assert.False(hasEb && hasGm);
    }

    [Fact]
    public async Task OptimizeAsync_ThrowsArgumentException_WhenTimeFromIsAfterTimeTo()
    {
        var optimizer = new Optimizer(assetProvider.Object, sourceProvider.Object, NullLogger<Optimizer>.Instance);
        var request = new OptimizationRequestDto { ScenarioId = 1, PeriodId = 1, TimeFrom = new DateTime(2026, 1, 2), TimeTo = new DateTime(2026, 1, 1) };

        await Assert.ThrowsAsync<ArgumentException>(() => optimizer.OptimizeAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task OptimizeAsync_HandlesException_FromAssetProvider()
    {
        assetProvider.Setup(x => x.GetAssetDataAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Asset Provider Failed"));

        var optimizer = new Optimizer(assetProvider.Object, sourceProvider.Object, NullLogger<Optimizer>.Instance);
        var request = new OptimizationRequestDto { ScenarioId = 1, PeriodId = 1, TimeFrom = new DateTime(2026, 1, 1), TimeTo = new DateTime(2026, 1, 2) };

        await Assert.ThrowsAsync<Exception>(() => optimizer.OptimizeAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task OptimizeAsync_ZeroDemand_ResultsInZeroExpenses()
    {
        var bundle = new AssetDataBundle
        {
            GasBoilers = new[] { new GasBoiler { Id = 1, Name = "GB1", MaxHeat = 20.0f, ProductionCost = 400f } }
        };

        assetProvider.Setup(x => x.GetAssetDataAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(bundle);

        sourceProvider.Setup(x => x.GetSourceDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SourceDataPoint>
            {
                new SourceDataPoint { PeriodId = 1, TimeFrom = new DateTime(2026, 1, 1, 10, 0, 0), TimeTo = new DateTime(2026, 1, 1, 11, 0, 0), HeatDemand = 0.0, ElectricityPrice = 100 }
            });

        var optimizer = new Optimizer(assetProvider.Object, sourceProvider.Object, NullLogger<Optimizer>.Instance);
        var request = new OptimizationRequestDto { ScenarioId = 1, PeriodId = 1, TimeFrom = new DateTime(2026, 1, 1), TimeTo = new DateTime(2026, 1, 2) };

        var result = await optimizer.OptimizeAsync(request, CancellationToken.None);

        Assert.Single(result.OptResultsHourly);
        Assert.Equal(0, result.OptResultsHourly[0].Expenses);
    }
}