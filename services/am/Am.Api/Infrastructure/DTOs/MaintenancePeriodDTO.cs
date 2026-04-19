using System;

namespace Am.Api.Infrastructure.DTOs;

public class MaintenancePeriodDTO
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? OptimizationRunId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsEmergency { get; set; }
    public string Status { get; set; } = string.Empty;
}
