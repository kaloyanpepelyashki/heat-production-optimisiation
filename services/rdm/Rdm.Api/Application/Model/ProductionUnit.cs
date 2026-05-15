namespace Rdm.Api.Application.Model;

public class ProductionUnit
{
    public int Id { get; set; }
    public int ProductionUnitId { get; set; }
    public string ProductionUnitType { get; set; }

    public double Expenses { get; set; }
    
    public double Co2Emissions { get; set; }

    public double HeatProduction { get; set; }

    public double ElectricityConsumption { get; set; }
    public double Capacity { get; set; }
}