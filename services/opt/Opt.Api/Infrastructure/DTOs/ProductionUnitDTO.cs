namespace Opt.Api.Infrastructure.DTOs;

public class ProductionUnitDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public float ProductionCost { get; set; }
    public bool Active { get; set; } = true;
}