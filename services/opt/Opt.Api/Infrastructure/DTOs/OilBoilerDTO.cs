namespace Opt.Api.Infrastructure.DTOs;

public class OilBoilerDTO : ProductionUnitDTO
{
    public float  MaxHeat { get; set; }
    public int Co2Emissions { get; set; }
    public float OilConsumption { get; set; }
}