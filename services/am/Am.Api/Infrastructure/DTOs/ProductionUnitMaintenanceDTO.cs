using System;

namespace Am.Api.Infrastructure.DTOs;

public class ProductionUnitMaintenanceDTO
{
    public int Id { get; set; }
    public int MaintenancePeriodId { get; set; }
    public int UnitTypeId { get; set; }
    public int UnitId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime TimeFrom { get; set; }
    public DateTime TimeTo { get; set; }
}