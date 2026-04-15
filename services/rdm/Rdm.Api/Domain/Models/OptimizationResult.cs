namespace Rdm.Api.Domain.Models;

public class OptimizationResult
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public double TotalCost { get; set; }
    public double TotalCo2Emissions { get; set; }
}