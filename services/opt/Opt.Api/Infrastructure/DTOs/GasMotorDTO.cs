namespace Am.Api.Infrastructure.DTOs;

public class GasMotorDTO
{
    public float  MaxHeat { get; set; }
    public float MaxElectricity { get; set; }
    public int Co2Emissions { get; set; }
    public float GasConsumption { get; set; }
}