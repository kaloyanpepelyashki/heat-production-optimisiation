using System;
using System.Collections.Generic;

namespace Dv.App.Models;

public sealed record OptimizationContext(
    string Period,
    string Scenario,
    DateTime StartDate,
    DateTime EndDate,
    IReadOnlyList<OptimizationBoilerDefinition> Boilers)
{
    public int PeriodId => string.Equals(Period, "Summer", StringComparison.OrdinalIgnoreCase) ? 1 : 2;

    public int ScenarioId => string.Equals(Scenario, "Scenario 1", StringComparison.OrdinalIgnoreCase) ? 1 : 2;
}

public sealed record OptimizationBoilerDefinition(string BoilerId, string FuelType);
