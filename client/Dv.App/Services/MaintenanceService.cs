namespace Dv.App.Services;

using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Avalonia.Threading;
using Dv.App.Models;
using Dv.App.Services;

public sealed class MaintenanceService
{
    private readonly IApiService apiService =  new ApiService();
    public async void SaveMaintenance(string boilerId, DateTime startDateTime, DateTime endDateTime, string period, string scenario)
    {
        var boilerMetadata = ParseBoilerMetadata(boilerId);
        var periodId = this.GetPeriodId(period);
        var scenarioId = GetScenarioId(scenario);

        // Print all selected maintenance input in one log line.
        ProductionUnitMaintenanceDTO productionUnitMaintenanceDTO = new ProductionUnitMaintenanceDTO
        {
            UnitId = boilerMetadata.BoilerId,
            UnitType = boilerId.Substring(0,2),
            CreatedAt = DateTime.UtcNow,
            FromDate = startDateTime,
            ToDate = endDateTime,
            PeriodId = int.Parse(periodId),
            ScenarioId = int.Parse(scenarioId),
        };
        int maintenanceId = await this.apiService.PostAsync<ProductionUnitMaintenanceDTO, int>(BackendService.Am, "api/GetProductionUnits/productionUnitMaintenance", productionUnitMaintenanceDTO);

        MaintenanceStore.MaintenanceSchedules.Add(new MaintenanceEvent
        {
            MaintenanceId = maintenanceId,
            AssetName = boilerId,
            BoilerId = boilerMetadata.BoilerId,
            BoilerType = boilerMetadata.BoilerType,
            StartDate = startDateTime,
            EndDate = endDateTime,
            Period = periodId,
            Scenario = scenarioId,
        });
    }

    public string GetPeriodId(string period)
    {
        return period.Equals("Summer", StringComparison.OrdinalIgnoreCase)
            ? "1"
            : period.Equals("Winter", StringComparison.OrdinalIgnoreCase)
                ? "2"
                : period;
    }

    private static string GetScenarioId(string scenario)
    {
        return scenario.Equals("Scenario 1", StringComparison.OrdinalIgnoreCase) || scenario == "1"
            ? "1"
            : scenario.Equals("Scenario 2", StringComparison.OrdinalIgnoreCase) || scenario == "2"
                ? "2"
                : scenario;
    }

    // Gets maintenance boiler information from selected boiler input
    private static (int BoilerId, string BoilerType) ParseBoilerMetadata(string boilerName)
    {
        var trimmedName = boilerName.Trim();
        var boilerType = trimmedName.StartsWith("GB", StringComparison.OrdinalIgnoreCase)
            ? "Gas"
            : trimmedName.StartsWith("OB", StringComparison.OrdinalIgnoreCase)
                ? "Oil"
                : string.Empty;

        var boilerIdText = Regex.Match(trimmedName, @"\d+").Value;
        _ = int.TryParse(boilerIdText, out var boilerId);

        return (boilerId, boilerType);
    }
}
