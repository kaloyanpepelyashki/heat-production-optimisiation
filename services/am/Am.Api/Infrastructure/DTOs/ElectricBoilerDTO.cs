namespace Am.Api.Infrastructure.DTOs;

public class ElectricBoilerDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public float  MaxHeat { get; set; }
    public float ProductionCost { get; set; }
    public float MaxElectricity { get; set; }
}