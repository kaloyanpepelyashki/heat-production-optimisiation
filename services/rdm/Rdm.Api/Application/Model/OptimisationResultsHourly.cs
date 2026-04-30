namespace Rdm.Api.Application.Model;

public class OptimisationResultsHourly
{
    public int Id { get; set; }
    public double HeatProduction { get; set; }
    public double ElectricityConsumption { get; set; }
    public double Co2Emissions { get; set; }
    public double Expenses { get; set; }
    public DateTime TimeFrom { get; set; }
    public DateTime TimeTo { get; set; }
    public List<ProductionUnit> ProductionUnits { get; set; }
}