using System;
using System.Collections.Generic;

namespace Dv.App.Models;

public static class OptimizationContextFactory
{
    public static OptimizationContext Create(string period, string scenario)
    {
        var normalizedPeriod = NormalizePeriod(period);
        var normalizedScenario = NormalizeScenario(scenario);

        var startDate = string.Equals(normalizedPeriod, "Winter", StringComparison.OrdinalIgnoreCase)
            ? new DateTime(2026, 1, 5, 0, 0, 0)
            : new DateTime(2025, 9, 8, 0, 0, 0);

        var endDate = string.Equals(normalizedPeriod, "Winter", StringComparison.OrdinalIgnoreCase)
            ? new DateTime(2026, 1, 19, 0, 0, 0)
            : new DateTime(2025, 9, 21, 23, 59, 59);

        IReadOnlyList<OptimizationBoilerDefinition> boilers =
            string.Equals(normalizedScenario, "Scenario 2", StringComparison.OrdinalIgnoreCase)
                ? new[]
                {
                    new OptimizationBoilerDefinition("GB1", "Gas"),
                    new OptimizationBoilerDefinition("GB3", "Gas"),
                    new OptimizationBoilerDefinition("GM1", "Gas"),
                    new OptimizationBoilerDefinition("EB1", "Electric"),
                }
                : new[]
                {
                    new OptimizationBoilerDefinition("GB1", "Gas"),
                    new OptimizationBoilerDefinition("GB2", "Gas"),
                    new OptimizationBoilerDefinition("GB3", "Gas"),
                    new OptimizationBoilerDefinition("OB1", "Oil"),
                };

        return new OptimizationContext(
            normalizedPeriod,
            normalizedScenario,
            startDate,
            endDate,
            boilers);
    }

    public static string NormalizePeriod(string period)
    {
        return string.Equals(period, "Winter", StringComparison.OrdinalIgnoreCase)
            ? "Winter"
            : "Summer";
    }

    public static string NormalizeScenario(string scenario)
    {
        if (string.Equals(scenario, "Scenario 2", StringComparison.OrdinalIgnoreCase)
            || string.Equals(scenario, "2", StringComparison.OrdinalIgnoreCase))
        {
            return "Scenario 2";
        }

        return "Scenario 1";
    }
}
