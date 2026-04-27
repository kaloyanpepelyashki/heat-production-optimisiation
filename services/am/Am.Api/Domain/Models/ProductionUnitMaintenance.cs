namespace Am.Api.Domain.Models;

public class ProductionUnitMaintenance
{
    public int Id;
    public int UnitTypeId;
    public int UnitId;
    public DateTime CreatedAt;
    public DateTime FromDate;
    public DateTime ToDate;
    public int PeriodId;
    public int ScenarioId;

}