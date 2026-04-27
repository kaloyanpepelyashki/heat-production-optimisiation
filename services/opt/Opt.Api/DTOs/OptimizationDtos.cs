namespace Opt.Api.DTOs;

public sealed class OptimizationRequestDto
{
    public bool IncludeMaintenanceSchedules { get; set; }
    public DateTime TimeFrom { get; set; }
    public DateTime TimeTo { get; set; }
}

public sealed class OptimizationResponseDto
{
    public string Status { get; set; } = string.Empty;
    public OptRunDto OptRun { get; set; } = new();
    public IReadOnlyList<OptResultsHourlyDto> OptResultsHourly { get; set; } = [];
}

public sealed class OptRunDto
{
    public int? Id { get; set; }
    public DateTime TimeFrom { get; set; }
    public DateTime TimeTo { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class OptResultsHourlyDto
{
    public int? Id { get; set; }
    public int? OptRunId { get; set; }
    public int PeriodId { get; set; }
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
    public int? Id { get; set; }
    public int? OptResultsHourlyId { get; set; }
    public string UnitType { get; set; } = string.Empty;
    public string UnitName { get; set; } = string.Empty;
    public double CapacityOutput { get; set; }
}