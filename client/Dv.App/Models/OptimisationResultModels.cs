namespace Dv.App.Models;

using System;
using System.Collections.Generic;

public class ApiResponseWrapper<T>
{
    public string? Message { get; set; }
    public string? Error { get; set; }
    public int? Count { get; set; }
    public T? Data { get; set; }
}

public class OptimisationRunClient
{
    public int Id { get; set; }
    public DateTime TimeFrom { get; set; }
    public DateTime TimeTo { get; set; }
    public string Scenario { get; set; } = string.Empty;
    public string PeriodType { get; set; } = string.Empty;
    public List<OptimisationResultsHourlyClient> OptimisationResultsHourly { get; set; } = new();
}

public class OptimisationResultsHourlyClient
{
    public int Id { get; set; }
    public double HeatProduction { get; set; }
    public double ElectricityConsumption { get; set; }
    public double Co2Emissions { get; set; }
    public double Expenses { get; set; }
    public DateTime TimeFrom { get; set; }
    public DateTime TimeTo { get; set; }
    public List<ProductionUnitClient> ProductionUnits { get; set; } = new();
}

public class ProductionUnitClient
{
    public int Id { get; set; }
    public int ProductionUnitId { get; set; }
    public string ProductionUnitType { get; set; } = string.Empty;
    public double HeatProduction { get; set; }
    public double ElectricityConsumption { get; set; }
    public double Expenses { get; set; }
    public double Co2Emissions { get; set; }
    public double Capacity { get; set; }
}
