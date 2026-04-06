namespace Opt.Api.Infrastructure.DTOs;

public class NetProductionCostDTO
{
    public int Id { get; set; }
    public int PeriodId { get; set; }
    public DateTime TimeFrom { get; set; }
    public DateTime TimeTo { get; set; }
    public double NetProductionCost { get; set; }
}