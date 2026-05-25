namespace Dv.App.Services;

using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dv.App.Interfaces;
using Dv.App.Models;

public sealed class MaintenanceService
{
    private readonly IApiService apiService;

    public MaintenanceService(IApiService apiService)
    {
        this.apiService = apiService;
    }

    public async Task SaveMaintenanceAsync(
        string boilerId,
        DateTime startDateTime,
        DateTime endDateTime,
        string period,
        string scenario)
    {
        var boilerMetadata = ParseBoilerMetadata(boilerId);
        var periodId = this.GetPeriodId(period);
        var scenarioId = GetScenarioId(scenario);

        var productionUnitMaintenanceDTO = new ProductionUnitMaintenanceDTO
        {
            UnitId = boilerMetadata.BoilerId,
            UnitType = boilerId.Substring(0, 2),
            CreatedAt = DateTime.UtcNow,
            FromDate = startDateTime,
            ToDate = endDateTime,
            PeriodId = int.Parse(periodId),
            ScenarioId = int.Parse(scenarioId),
        };

        var newId = await this.apiService.PostAsync<ProductionUnitMaintenanceDTO, int>(
            BackendService.Am,
            "api/GetProductionUnits/productionUnitMaintenance",
            productionUnitMaintenanceDTO);

        MaintenanceStore.MaintenanceSchedules.Add(new MaintenanceEvent
        {
            AssetName = boilerId,
            BoilerId = boilerMetadata.BoilerId,
            MaintenanceId = newId,
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

    private static (int BoilerId, string BoilerType) ParseBoilerMetadata(string boilerName)
    {
        var trimmedName = boilerName.Trim();
        var boilerType = trimmedName.StartsWith("GB", StringComparison.OrdinalIgnoreCase)
            || trimmedName.StartsWith("GM", StringComparison.OrdinalIgnoreCase)
                ? "Gas"
                : trimmedName.StartsWith("OB", StringComparison.OrdinalIgnoreCase)
                    ? "Oil"
                    : trimmedName.StartsWith("EB", StringComparison.OrdinalIgnoreCase)
                        ? "Electric"
                        : string.Empty;

        var boilerIdText = Regex.Match(trimmedName, @"\d+").Value;
        _ = int.TryParse(boilerIdText, out var boilerId);

        return (boilerId, boilerType);
    }
}
