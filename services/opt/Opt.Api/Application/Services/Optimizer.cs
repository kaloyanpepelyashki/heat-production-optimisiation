
using Opt.Api.Application.Interfaces;
using Opt.Api.DTOs;
using Opt.Api.Domain.Models;
using Opt.Api.Application.Exceptions;
using Microsoft.Extensions.Logging;
namespace Opt.Api.Application.Services;

public class Optimizer
{
    private readonly IAssetDataProvider _assetDataProvider;
    private readonly ISourceDataProvider _sourceDataProvider;

    private readonly ILogger<Optimizer> _logger;

    public Optimizer(IAssetDataProvider assetDataProvider, ISourceDataProvider sourceDataProvider, ILogger<Optimizer> logger)
    {
        _assetDataProvider = assetDataProvider;
        _sourceDataProvider = sourceDataProvider;
        _logger = logger;
    }

    public async Task<OptimizationResponseDto> OptimizeAsync(
        OptimizationRequestDto request,
        CancellationToken cancellationToken)
    {

        try {
            
        var assets = await _assetDataProvider.GetAssetDataAsync(request.MaintenanceId, cancellationToken);
        var sourceDataResponse = await _sourceDataProvider.GetSourceDataAsync(cancellationToken);

        var sourceData = sourceDataResponse
            .Where(x => x.PeriodId == request.PeriodId)
            .Where(x => x.TimeFrom >= request.TimeFrom && x.TimeTo <= request.TimeTo)
            .OrderBy(x => x.TimeFrom)
            .ToList();

        var boilers = request.ScenarioId == 2 
            ? BuildScenario2Boilers(assets)
            : BuildScenario1Boilers(assets);
        
        var createdAt = DateTime.UtcNow;
        var hourlyResults = sourceData
            .Select(point => BuildHourlyResult(point, assets, boilers))
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
        catch (Exception ex) when (ex is not OperationCanceledException and not InvalidOperationException)
        {
            throw new ExternalDataFetchException("Optimization failed.", ex);
        }
    }

    private static List<DispatchUnit> BuildScenario1Boilers(AssetDataBundle assets)
    {
        var boilers = new List<DispatchUnit>();

        boilers.AddRange(assets.GasBoilers
            .Select(x => new DispatchUnit(
                x.Id,
                "GB",
                x.Name,
                x.MaxHeat,
                x.ProductionCost,
                x.GasConsumption,
                x.Co2Emissions,
                0d)));

        boilers.AddRange(assets.OilBoilers
            .Select(x => new DispatchUnit(
                x.Id,
                "OB",
                x.Name,
                x.MaxHeat,
                x.ProductionCost,
                x.OilConsumption,
                x.Co2Emissions,
                0d)));

        return boilers;
    }

    private static List<DispatchUnit> BuildScenario2Boilers(AssetDataBundle assets)
    {
        var boilers = new List<DispatchUnit>();

        boilers.AddRange(assets.GasBoilers
            .Where(x => x.Name != "GB2")
            .Select(x => new DispatchUnit(
                x.Id,
                "GB",
                x.Name,
                x.MaxHeat,
                x.ProductionCost,
                x.GasConsumption,
                x.Co2Emissions,
                0d)));

        boilers.AddRange(assets.ElectricBoilers
            .Select(x => new DispatchUnit(
                x.Id,
                "EB",
                x.Name,
                x.MaxHeat,
                x.ProductionCost,
                0d,
                0d,
                x.MaxElectricity)));

        boilers.AddRange(assets.GasMotors
            .Select(x => new DispatchUnit(
                x.Id,
                "GM",
                x.Name,
                x.MaxHeat,
                x.ProductionCost,
                x.GasConsumption,
                x.Co2Emissions,
                x.MaxElectricity)));

        return boilers;
    }

    private static OptResultsHourlyDto BuildHourlyResult(
        SourceDataPoint point,
        AssetDataBundle assets,
        IReadOnlyList<DispatchUnit> boilers)
    {
        var maintenance = assets.MaintenanceSchedule;
        
        var availableBoilers = boilers
            .Where(
            b => maintenance is null ||
            !(maintenance.UnitType == b.UnitType &&
            maintenance.UnitId == b.UnitId &&
            maintenance.FromDate <= point.TimeFrom &&
            maintenance.ToDate >= point.TimeTo))
            .OrderBy(b => b.GetCostPerHeat(point.ElectricityPrice))
            .ToList();

        var remainingHeatDemand = Math.Max(0d, point.HeatDemand);
        var unitRows = new List<PUnitDto>();
        var netCost = 0d;
        var co2 = 0d;
        var netElectricity = 0d;

        var isEbDispatched = false;
        var isGmDispatched = false;

        foreach (var boiler in availableBoilers)
        {
            if (remainingHeatDemand <= 0d)
            {
                break;
            }

            if (boiler.UnitType == "EB" && isGmDispatched)
            {
                continue;
            }
            if (boiler.UnitType == "GM" && isEbDispatched)
            {
                continue;
            }

            var dispatchedHeat = Math.Min(boiler.MaxHeat, remainingHeatDemand);

            var loadRatio = dispatchedHeat / boiler.MaxHeat;
            
            netCost += boiler.GetExpensesAtFull(point.ElectricityPrice) * loadRatio;
            co2 += boiler.Co2PerMWh * dispatchedHeat;
            
            if (boiler.UnitType == "EB")
            {
                netElectricity -= boiler.MaxElectricity * loadRatio;
                isEbDispatched = true;
            }
            else if (boiler.UnitType == "GM")
            {
                netElectricity -= boiler.MaxElectricity * loadRatio;
                isGmDispatched = true;
            }

            unitRows.Add(new PUnitDto
            {
                UnitType = boiler.UnitType,
                UnitId = boiler.UnitId,
                HeatProduction = Math.Round(dispatchedHeat, 2),
                ElectricityConsumption = Math.Round(
                    (boiler.UnitType == "EB" || boiler.UnitType == "GM") ? boiler.MaxElectricity * loadRatio * -1 : 0d, 2),
                Expenses = Math.Round(boiler.GetExpensesAtFull(point.ElectricityPrice) * loadRatio, 2),
                Co2Emissions = Math.Round(boiler.Co2PerMWh * dispatchedHeat, 2),
                CapacityOutput = Math.Round(loadRatio * 100d, 2),
            });

            remainingHeatDemand -= dispatchedHeat;
        }

        if (remainingHeatDemand > 0d)
        {
            throw new InvalidOperationException("Heat demand could not be fully covered with available boilers.");
        }

        return new OptResultsHourlyDto
        {
            HeatProduction = Math.Round(point.HeatDemand, 2),
            ElectricityConsumption = Math.Round(netElectricity, 2),
            Expenses = Math.Round(netCost, 2),
            Co2Emissions = Math.Round(co2, 2),
            TimeFrom = point.TimeFrom,
            TimeTo = point.TimeTo,
            Units = unitRows,
        };
    }

    private sealed record DispatchUnit(
        int UnitId,
        string UnitType,
        string UnitName,
        double MaxHeat,
        double ProductionCost,
        double FuelConsumption,
        double Co2PerMWh,
        double MaxElectricity)
    {
        public double GetExpensesAtFull(double electricityPrice)
        {
            double baseCost = ProductionCost * MaxHeat;
            if (UnitType == "EB")
                return baseCost - (MaxElectricity * electricityPrice);
            if (UnitType == "GM")
                return baseCost - (MaxElectricity * electricityPrice);
            return baseCost;
        }

        public double GetCostPerHeat(double electricityPrice) => GetExpensesAtFull(electricityPrice) / MaxHeat;
    }
}