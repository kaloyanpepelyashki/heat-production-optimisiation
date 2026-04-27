
using Opt.Api.Application.Interfaces;
using Opt.Api.DTOs;
using Opt.Api.Domain.Models;

namespace Opt.Api.Application.Services;

public class Optimizer
{
    private readonly IAssetDataProvider _assetDataProvider;
    private readonly ISourceDataProvider _sourceDataProvider;

    public Optimizer(IAssetDataProvider assetDataProvider, ISourceDataProvider sourceDataProvider)
    {
        _assetDataProvider = assetDataProvider;
        _sourceDataProvider = sourceDataProvider;
    }

    public async Task<OptimizationResponseDto> OptimizeAsync(
        OptimizationRequestDto request,
        int periodId,
        CancellationToken cancellationToken)
    {

        var assetsTask = _assetDataProvider.GetAssetDataAsync(cancellationToken);
        var sourceDataTask = _sourceDataProvider.GetSourceDataAsync(cancellationToken);

        await Task.WhenAll(assetsTask, sourceDataTask);

        var assets = await assetsTask;
        var sourceData = (await sourceDataTask)
            .Where(x => x.PeriodId == periodId)
            .Where(x => x.TimeFrom >= request.TimeFrom && x.TimeTo <= request.TimeTo)
            .OrderBy(x => x.TimeFrom)
            .ToList();

        var createdAt = DateTime.UtcNow;
        var sortedBoilers = BuildScenario1Boilers(assets);
        var hourlyResults = sourceData
            .Select(point => BuildHourlyResult(point, sortedBoilers, createdAt))
            .ToList();

        var runFrom = sourceData.Count == 0 ? createdAt : sourceData.Min(x => x.TimeFrom);
        var runTo = sourceData.Count == 0 ? createdAt : sourceData.Max(x => x.TimeTo);

        return new OptimizationResponseDto
        {
            Status = "Boiler-only optimization completed for selected period.",
            OptRun = new OptRunDto
            {
                Id = null,
                TimeFrom = runFrom,
                TimeTo = runTo,
                CreatedAt = createdAt,
            },
            OptResultsHourly = hourlyResults,
        };
    }

    private static List<DispatchBoiler> BuildScenario1Boilers(AssetDataBundle assets)
    {
        var boilers = new List<DispatchBoiler>();

        boilers.AddRange(assets.GasBoilers
            .Where(x => x.MaxHeat > 0f)
            .Select(x => new DispatchBoiler(
                x.Id,
                "GasBoiler",
                x.Name,
                x.MaxHeat,
                x.ProductionCost,
                x.GasConsumption,
                x.Co2Emissions)));

        boilers.AddRange(assets.OilBoilers
            .Where(x => x.MaxHeat > 0f)
            .Select(x => new DispatchBoiler(
                x.Id,
                "OilBoiler",
                x.Name,
                x.MaxHeat,
                x.ProductionCost,
                x.OilConsumption,
                x.Co2Emissions)));

        return boilers
            .OrderBy(x => x.CostPerHeat)
            .ToList();
    }

    private static OptResultsHourlyDto BuildHourlyResult(
        SourceDataPoint point,
        IReadOnlyList<DispatchBoiler> sortedBoilers,
        DateTime createdAt)
    {
        var remainingHeatDemand = Math.Max(0d, point.HeatDemand);
        var unitRows = new List<PUnitDto>();

        foreach (var boiler in sortedBoilers)
        {
            if (remainingHeatDemand <= 0d)
            {
                break;
            }

            var dispatchedHeat = Math.Min(boiler.MaxHeat, remainingHeatDemand);
            if (dispatchedHeat <= 0d)
            {
                continue;
            }

            var loadRatio = dispatchedHeat / boiler.MaxHeat;

            unitRows.Add(new PUnitDto
            {
                Id = null,
                OptResultsHourlyId = null,
                UnitType = boiler.UnitType,
                UnitName = boiler.UnitName,
                CapacityOutput = loadRatio * 100d,
            });

            remainingHeatDemand -= dispatchedHeat;
        }

        if (remainingHeatDemand > 0d)
        {
            throw new InvalidOperationException("Heat demand could not be fully covered with available boilers.");
        }

        return new OptResultsHourlyDto
        {
            Id = null,
            OptRunId = null,
            PeriodId = point.PeriodId,
            HeatProduction = point.HeatDemand,
            ElectricityConsumption = 0d,
            Expenses = CalculateHourlyExpenses(point.HeatDemand, sortedBoilers),
            Co2Emissions = CalculateHourlyCo2(point.HeatDemand, sortedBoilers),
            TimeFrom = point.TimeFrom,
            TimeTo = point.TimeTo,
            Units = unitRows,
        };
    }

    private static double CalculateHourlyExpenses(double heatDemand, IReadOnlyList<DispatchBoiler> sortedBoilers)
    {
        var remainingHeatDemand = Math.Max(0d, heatDemand);
        var expenses = 0d;

        foreach (var boiler in sortedBoilers)
        {
            var dispatchedHeat = Math.Min(boiler.MaxHeat, remainingHeatDemand);
            if (dispatchedHeat <= 0d)
            {
                continue;
            }

            var loadRatio = dispatchedHeat / boiler.MaxHeat;
            expenses += boiler.FullLoadExpense * loadRatio;
            remainingHeatDemand -= dispatchedHeat;
        }

        return expenses;
    }

    private static double CalculateHourlyCo2(double heatDemand, IReadOnlyList<DispatchBoiler> sortedBoilers)
    {
        var remainingHeatDemand = Math.Max(0d, heatDemand);
        var co2 = 0d;

        foreach (var boiler in sortedBoilers)
        {
            if (remainingHeatDemand <= 0d)
            {
                break;
            }

            var dispatchedHeat = Math.Min(boiler.MaxHeat, remainingHeatDemand);
            if (dispatchedHeat <= 0d)
            {
                continue;
            }

            var loadRatio = dispatchedHeat / boiler.MaxHeat;
            co2 += boiler.FullLoadCo2 * loadRatio;
            remainingHeatDemand -= dispatchedHeat;
        }

        return co2;
    }

    private sealed record DispatchBoiler(
        int UnitExternalId,
        string UnitType,
        string UnitName,
        double MaxHeat,
        double ProductionCost,
        double FuelConsumption,
        double FullLoadCo2)
    {
        public double FullLoadExpense => ProductionCost * FuelConsumption;

        // For dispatch ordering, use cost normalized by full heat output.
        public double CostPerHeat => MaxHeat <= 0d ? double.MaxValue : FullLoadExpense / MaxHeat;
    }
}