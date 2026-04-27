namespace Dv.App.Services;

using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Dv.App.Models;

public sealed class MaintenanceService
{
    public void SaveMaintenance(string boilerId, DateTime startDateTime, DateTime endDateTime, string period, string scenario)
    {
        var boilerMetadata = ParseBoilerMetadata(boilerId);
        var periodId = this.GetPeriodId(period);
        var scenarioId = GetScenarioId(scenario);

        // Print all selected maintenance input in one log line.
        Debug.WriteLine(
            $"Maintenance input -> Boiler: {boilerId}, BoilerId: {boilerMetadata.BoilerId}, BoilerType: {boilerMetadata.BoilerType}, Start: {startDateTime:yyyy-MM-dd HH:mm}, End: {endDateTime:yyyy-MM-dd HH:mm}, Period: {periodId}, Scenario: {scenarioId}");

        MaintenanceStore.MaintenanceSchedules.Add(new MaintenanceEvent
        {
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
