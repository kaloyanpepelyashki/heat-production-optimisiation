namespace Rdm.Api.Infrastructure.DTOs;

public class OptimizationResultDTO
{
    public int Id { get; set; }

    public string ProductionUnit { get; set; } = string.Empty;

    public string Period { get; set; } = string.Empty;

    public float TotalHeat { get; set; }

    public float TotalCost { get; set; }

    public float TotalEmissions { get; set; }

    public DateTime CreatedAt { get; set; }
}

public class CreateOptimizationResultDTO
{
    public string ProductionUnit { get; set; } = string.Empty;

    public string Period { get; set; } = string.Empty;

    public float TotalHeat { get; set; }

    public float TotalCost { get; set; }

    public float TotalEmissions { get; set; }
}