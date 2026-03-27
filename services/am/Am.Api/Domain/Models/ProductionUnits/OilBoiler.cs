namespace Am.Api.Domain.Models;

/// <summary>
/// Model for the Oil Boiler.
/// </summary>

public class OilBoiler : ProductionUnit
{
    public int Co2Emissions;
    public float OilConsumption;
}
