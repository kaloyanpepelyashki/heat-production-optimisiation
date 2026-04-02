namespace Am.Api.Infrastructure.DTOs;

public class GasMotorDTO : IConsumptionDTO, IProductionCostDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public float  MaxHeat { get; set; }
    public float MaxElectricity { get; set; }
    public float ProductionCost { get; set; }
    public int Co2Emissions { get; set; }
    public float GasConsumption { get; set; }

    public float Consumption => GasConsumption;
}