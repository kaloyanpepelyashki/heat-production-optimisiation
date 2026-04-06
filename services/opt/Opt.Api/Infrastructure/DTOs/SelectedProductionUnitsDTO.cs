namespace Opt.Api.Infrastructure.DTOs;

public class SelectedProductionUnitsDTO
{
    public int Id { get; set; }
    public int PeriodId { get; set; }
    public DateTime TimeFrom { get; set; }
    public DateTime TimeTo { get; set; }
    public List<string> SelectedProductionUnitsNames { get; set; }
}