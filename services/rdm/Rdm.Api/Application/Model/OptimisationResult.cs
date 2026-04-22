namespace Rdm.Api.Application.Model;

//TODO - To be updated, after the database schema is changed
public class OptimisationResult
{
    public int Id { get; set; }
    public double HeatProduction { get; set; }
    public double ElectricityConsumption { get; set; }
    public double Expenses { get; set; }
    public double Profit { get; set; }
    public double ProducedCo2Emissions { get; set; }
    public DateTime DateRun { get; set; }
    public List<ProductionUnit> ProductionUnits { get; set; }
}
