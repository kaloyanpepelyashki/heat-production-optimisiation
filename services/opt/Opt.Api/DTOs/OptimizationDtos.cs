namespace Opt.Api.DTOs;

public sealed class OptimizationRequestDto
{
    public int ScenarioId { get; set; }
    
    public int PeriodId { get; set; }
    public int MaintenanceId { get; set; }
    public DateTime TimeFrom { get; set; }
    public DateTime TimeTo { get; set; }
}

public sealed class OptimizationResponseDto
{
    public OptRunDto OptRun { get; set; } = new();
    public IReadOnlyList<OptResultsHourlyDto> OptResultsHourly { get; set; } = [];
}

public sealed class OptRunDto
{
    public DateTime TimeFrom { get; set; }
    public DateTime TimeTo { get; set; }

    public string Scenario { get; set; } = string.Empty;

    public string PeriodType { get; set; } = string.Empty;
}

public sealed class OptResultsHourlyDto
{
    public double HeatProduction { get; set; }
    public double ElectricityConsumption { get; set; }
    public double Expenses { get; set; }
    public double Co2Emissions { get; set; }
    public DateTime TimeFrom { get; set; }
    public DateTime TimeTo { get; set; }
    public IReadOnlyList<PUnitDto> Units { get; set; } = [];
}

public sealed class PUnitDto
{
    public string UnitType { get; set; } = string.Empty;
    public int UnitId { get; set; } 
    public double HeatProductionPerUnit { get; set; }
    public double ElectricityConsumptionPerUnit { get; set; }
    public double ExpensesPerUnit { get; set; }
    public double Co2EmissionsPerUnit { get; set; } 
    public double CapacityOutput { get; set; }
}