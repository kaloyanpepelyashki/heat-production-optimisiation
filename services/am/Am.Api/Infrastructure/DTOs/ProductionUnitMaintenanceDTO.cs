using System;

namespace Am.Api.Infrastructure.DTOs;

public class ProductionUnitMaintenanceDTO
{
    public int Id { get; set; }
    public string UnitType { get; set; }
    public int UnitId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int PeriodId {get; set;}
    public int ScenarioId {get; set;}
}