namespace Rdm.Api.Application.Model;

public class OptimisationResultsHourly
{
    public int Id { get; set; }
    public float HeatProduction { get; set; }
    public float ElectricityConsumption { get; set; }
    public float Co2Emissions { get; set; }
    public float Expenses { get; set; }
    public DateTime TimeFrom { get; set; }
    public DateTime TimeTo { get; set; }
    public List<ProductionUnit> ProductionUnits { get; set; }
}