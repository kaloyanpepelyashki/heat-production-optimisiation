namespace Am.Api.Domain.Models;

/// <summary>
/// Model for the Gas Boiler.
/// </summary>n

public class GasBoiler : ProductionUnit
{
    public int? CO2Emission;
    public double? GasConsumption;
}

