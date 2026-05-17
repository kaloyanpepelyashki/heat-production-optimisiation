namespace Dv.App.Models;

using System;

public class OptimisationRequestDto
{
    public int ScenarioId { get; set; }
    public int PeriodId { get; set; }
    public int MaintenanceId { get; set; }
    public DateTime TimeFrom { get; set; }
    public DateTime TimeTo { get; set; }
}
