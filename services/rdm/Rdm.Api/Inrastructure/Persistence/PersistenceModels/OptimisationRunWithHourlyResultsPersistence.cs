namespace Rdm.Api.Inrastructure.Persistence.PersistenceModels;

public class OptimisationRunWithHourlyResultsPersistence : OptimisationRunPersistence
{
    public List<OptimisationResultHourlyWithProductionUnitPersistence> OptimisationResultsHourly { get; set; }
}