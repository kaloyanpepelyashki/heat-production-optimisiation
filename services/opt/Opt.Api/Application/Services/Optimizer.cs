using Opt.Api.Application.Interfaces;
using Opt.Api.DTOs;
using Opt.Api.Domain.Models;
using Opt.Api.Application.Exceptions;

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
        CancellationToken cancellationToken)
    {
        try
        {
            // Ping services to wake them up
            await _assetDataProvider.PingAsync(cancellationToken);
            await _sourceDataProvider.PingAsync(cancellationToken);

            await Task.Delay(2000, cancellationToken);
            
            var assets = await _assetDataProvider.GetAssetDataAsync(request.MaintenanceId, cancellationToken);
            var sourceData = (await _sourceDataProvider.GetSourceDataAsync(cancellationToken))
                .Where(x => x.PeriodId == request.PeriodId)
                .Where(x => x.TimeFrom >= request.TimeFrom && x.TimeTo <= request.TimeTo)
                .OrderBy(x => x.TimeFrom)
                .ToList();

            var createdAt = DateTime.UtcNow;

            var hourlyResults = sourceData
                .Select(point => BuildHourlyResult(point, assets, request.ScenarioId, createdAt))
                .ToList();

            var runFrom = sourceData.Count == 0 ? createdAt : sourceData.Min(x => x.TimeFrom);
            var runTo = sourceData.Count == 0 ? createdAt : sourceData.Max(x => x.TimeTo);

            return new OptimizationResponseDto
            {
                OptRun = new OptRunDto
                {
                    TimeFrom = runFrom,
                    TimeTo = runTo,
                    Scenario = request.ScenarioId.ToString(),
                    PeriodType = request.PeriodId switch
                    {
                        1 => "summer",
                        2 => "winter",
                        _ => request.PeriodId.ToString()
                    },
                },
                OptResultsHourly = hourlyResults,
            };
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new ExternalDataFetchException("Optimization failed.", ex);
        }
    }

    private static List<DispatchUnit> BuildUnits(AssetDataBundle assets, SourceDataPoint point, int scenarioId)
    {
        var boilers = new List<DispatchUnit>();

        switch (scenarioId)
        {
            case 1:

                boilers.AddRange(assets.GasBoilers.Select(x => new DispatchUnit(
                    x.Id, 
                    "GB", 
                    x.Name, 
                    x.MaxHeat, 
                    0d,
                    x.ProductionCost, 
                    x.GasConsumption, 
                    x.Co2Emissions)));

                boilers.AddRange(assets.OilBoilers.Select(x => new DispatchUnit(
                    x.Id, 
                    "OB", 
                    x.Name, 
                    x.MaxHeat, 
                    0d,
                    x.ProductionCost, 
                    x.OilConsumption, 
                    x.Co2Emissions)));

                return boilers.OrderBy(x => x.ProductionCost).ToList();

            case 2:

                boilers.AddRange(assets.GasBoilers.Select(x => new DispatchUnit(
                    x.Id, 
                    "GB", 
                    x.Name, 
                    x.MaxHeat, 
                    0d,
                    x.ProductionCost, 
                    x.GasConsumption, 
                    x.Co2Emissions)));

                boilers.AddRange(assets.ElectricBoilers.Select(x => new DispatchUnit(
                    x.Id, 
                    "EB", 
                    x.Name, 
                    x.MaxHeat, 
                    x.MaxElectricity,
                    x.ProductionCost, 
                    0d, 
                    0d)));

                boilers.AddRange(assets.GasMotors.Select(x => new DispatchUnit(
                    x.Id, 
                    "GM", 
                    x.Name, 
                    x.MaxHeat, 
                    x.MaxElectricity,
                    x.ProductionCost, 
                    x.GasConsumption, 
                    x.Co2Emissions)));

                return boilers
                    .OrderBy(x => x.GetEffectiveHeatCost(point.ElectricityPrice))
                    .ToList();

            default:
                throw new ArgumentException($"Unsupported scenario ID: {scenarioId}");
        }
    }

    private static OptResultsHourlyDto BuildHourlyResult(
        SourceDataPoint point,
        AssetDataBundle assets,
        int scenarioId,
        DateTime createdAt)
    {
        var sortedBoilers = BuildUnits(assets, point, scenarioId);

        var maintenance = assets.MaintenanceSchedule;

        var availableBoilers = sortedBoilers
            .Where(b =>
                maintenance is null ||
                !(maintenance.UnitType == b.UnitType &&
                  maintenance.UnitId == b.UnitId &&
                  maintenance.FromDate <= point.TimeFrom &&
                  maintenance.ToDate >= point.TimeTo))
            .ToList();

        var remainingHeatDemand = Math.Max(0d, point.HeatDemand);
        var unitRows = new List<PUnitDto>();
        var netElectricityConsumption = 0d;

        foreach (var boiler in availableBoilers)
        {
            if (remainingHeatDemand <= 0d)
                break;

            var dispatchedHeat = Math.Min(boiler.MaxHeat, remainingHeatDemand);
            var loadRatio = dispatchedHeat / boiler.MaxHeat;

            unitRows.Add(new PUnitDto
            {
                UnitType = boiler.UnitType,
                UnitId = boiler.UnitId,
                CapacityOutput = Math.Round(loadRatio * 100d, 2),
            });

            netElectricityConsumption -= boiler.MaxElectricity * loadRatio;

            remainingHeatDemand -= dispatchedHeat;
        }

        if (remainingHeatDemand > 0d)
            throw new InvalidOperationException("Heat demand could not be fully covered with available boilers.");

        return new OptResultsHourlyDto
        {
            HeatProduction = point.HeatDemand,
            ElectricityConsumption = Math.Round(netElectricityConsumption, 2),
            Expenses = Math.Round(CalculateHourlyExpenses(point.HeatDemand, availableBoilers, point.ElectricityPrice), 2),
            Co2Emissions = Math.Round(CalculateHourlyCo2(point.HeatDemand, availableBoilers), 2),
            TimeFrom = point.TimeFrom,
            TimeTo = point.TimeTo,
            Units = unitRows,
        };
    }

    private static double CalculateHourlyExpenses(
        double heatDemand,
        IReadOnlyList<DispatchUnit> sortedBoilers,
        double electricityPrice)
    {
        var remainingHeatDemand = Math.Max(0d, heatDemand);
        var expenses = 0d;

        foreach (var boiler in sortedBoilers)
        {
            if (remainingHeatDemand <= 0d)
                break;

            var dispatchedHeat = Math.Min(boiler.MaxHeat, remainingHeatDemand);
            if (dispatchedHeat <= 0d)
                continue;

            var netCostPerMWh = boiler.GetEffectiveHeatCost(electricityPrice);

            var unitExpense = netCostPerMWh * dispatchedHeat;
            expenses += unitExpense;

            remainingHeatDemand -= dispatchedHeat;
        }

        return expenses;
    }

    private static double CalculateHourlyCo2(double heatDemand, IReadOnlyList<DispatchUnit> sortedBoilers)
    {
        var remainingHeatDemand = Math.Max(0d, heatDemand);
        var co2 = 0d;

        foreach (var boiler in sortedBoilers)
        {
            if (remainingHeatDemand <= 0d)
                break;

            var dispatchedHeat = Math.Min(boiler.MaxHeat, remainingHeatDemand);
            
            co2 += boiler.Co2PerMWh * dispatchedHeat;

            remainingHeatDemand -= dispatchedHeat;
        }

        return co2;
    }

    private sealed record DispatchUnit(
        int UnitId,
        string UnitType,
        string UnitName,
        double MaxHeat,
        double MaxElectricity,
        double ProductionCost,
        double FuelConsumption,
        double Co2PerMWh)
    {
        public double GetEffectiveHeatCost(double electricityPrice)
        {
            return UnitType switch
            {
                "EB" => ProductionCost + electricityPrice,
                "GM" => ProductionCost - ((MaxElectricity / MaxHeat) * electricityPrice),
                _ => ProductionCost,
            };
        }
    }
}