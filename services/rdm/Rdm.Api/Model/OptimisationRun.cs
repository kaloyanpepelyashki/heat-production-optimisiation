namespace Rdm.Api.Application.Model;

public class OptimisationRun
{
    public int Id { get; set; }

    public DateTime TimeFrom { get; set; }

    public DateTime TimeTo { get; set; }

    public string Scenario { get; set; }

    public string PeriodType { get; set; }

    public List<OptimisationResultsHourly> OptimisationResultsHourly { get; set; }
}
