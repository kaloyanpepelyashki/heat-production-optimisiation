namespace Am.Api.Domain.Models;

/// <summary>
/// Model for the Gs Motor.
/// </summary>n

public class GasMotor : ProductionUnit
{
    public double? MaxElectricity;
    public int? CO2Emission;
    public double? GasConsumption;

    public GasMotor()
    {
        type = this.GetType().Name.ToLower();
    }
}