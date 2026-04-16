namespace Rdm.Api.Domain.Models;

public class OptimizationResult
{
    public int Id { get; set; }

    public string ProductionUnit { get; set; } = string.Empty;

    public float TotalHeat { get; set; }

    public float TotalCost { get; set; }

    public float TotalEmissions { get; set; }

    public DateTime CreatedAt { get; set; }
}