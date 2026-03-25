namespace Am.Api.Domain.Models;

/// <summary>
/// Model for the Oil Boiler.
/// </summary>

public class OilBoiler : ProductionUnit
{
    public int? CO2Emission;
    public double? OilConsumption;
}

