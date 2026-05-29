namespace Dv.App.Models;

using System;
using System.Collections.Generic;

public sealed record OptimizationContext(
    string Period,
    string Scenario,
    DateTime StartDate,
    DateTime EndDate,
    IReadOnlyList<OptimizationBoilerDefinition> Boilers)
{
    public int PeriodId => string.Equals(this.Period, "Summer", StringComparison.OrdinalIgnoreCase) ? 1 : 2;

    public int ScenarioId => string.Equals(this.Scenario, "Scenario 1", StringComparison.OrdinalIgnoreCase) ? 1 : 2;
}

public sealed record OptimizationBoilerDefinition(string BoilerId, string FuelType);
