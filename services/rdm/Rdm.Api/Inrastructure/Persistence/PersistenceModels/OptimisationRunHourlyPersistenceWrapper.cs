namespace Rdm.Api.Inrastructure.Persistence.PersistenceModels;

public class OptimisationResultsHourlyPersistenceWrapper
{
    public OptimisationResultsHourlyPersistence HourlyResult { get; set; }

    public List<OptimisationProductionUnitPersistence> ProductionUnitsPersistence { get; set; }
}