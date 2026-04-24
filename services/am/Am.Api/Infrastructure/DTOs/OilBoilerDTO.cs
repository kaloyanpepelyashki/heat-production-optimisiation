namespace Am.Api.Infrastructure.DTOs;

public class OilBoilerDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public float  MaxHeat { get; set; }
    public float ProductionCost { get; set; }
    public int Co2Emissions { get; set; }
    public float OilConsumption { get; set; }
    public float Consumption => OilConsumption;
    public bool Active { get; set; } = true;
}