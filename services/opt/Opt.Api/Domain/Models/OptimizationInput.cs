namespace Opt.Api.Domain.Models;

public sealed class AssetDataBundle
{
    public IReadOnlyList<GasBoiler> GasBoilers { get; init; } = [];
    public IReadOnlyList<OilBoiler> OilBoilers { get; init; } = [];
    public IReadOnlyList<ElectricBoiler> ElectricBoilers { get; init; } = [];
    public IReadOnlyList<GasMotor> GasMotors { get; init; } = [];
    public IReadOnlyList<MaintenanceSchedule> MaintenanceSchedules { get; init; } = [];
}

public sealed class GasBoiler
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public float MaxHeat { get; init; }
    public float ProductionCost { get; init; }
    public int Co2Emissions { get; init; }
    public float GasConsumption { get; init; }
}

public sealed class OilBoiler
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public float MaxHeat { get; init; }
    public float ProductionCost { get; init; }
    public int Co2Emissions { get; init; }
    public float OilConsumption { get; init; }
}

public sealed class ElectricBoiler
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public float MaxHeat { get; init; }
    public float ProductionCost { get; init; }
    public float MaxElectricity { get; init; }
}

public sealed class GasMotor
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public float MaxHeat { get; init; }
    public float MaxElectricity { get; init; }
    public float ProductionCost { get; init; }
    public int Co2Emissions { get; init; }
    public float GasConsumption { get; init; }
}

public sealed class MaintenanceSchedule
{
    public int UnitId { get; init; }
    public DateTime StartUtc { get; init; }
    public DateTime EndUtc { get; init; }
}

public sealed class SourceDataPoint
{
    public int Id { get; init; }
    public int PeriodId { get; init; }
    public DateTime TimeFrom { get; init; }
    public DateTime TimeTo { get; init; }
    public double HeatDemand { get; init; }
    public double ElectricityPrice { get; init; }
}

public sealed class OptimizationInput
{
    public AssetDataBundle Assets { get; init; } = new();
    public IReadOnlyList<SourceDataPoint> SourceData { get; init; } = [];
    public bool IncludeMaintenanceSchedules { get; init; }
}