namespace Am.Api.Domain.Models;

/// <summary>
/// Model for the Electric Boiler.
/// </summary>n
public class ElectricBoiler : ProductionUnit
{
    public double? MaxElectricity;

    public ElectricBoiler()
    {
        type = this.GetType().Name.ToLower();
    }
}