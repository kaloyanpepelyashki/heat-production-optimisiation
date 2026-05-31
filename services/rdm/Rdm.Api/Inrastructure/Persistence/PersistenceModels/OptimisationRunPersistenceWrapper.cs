namespace Rdm.Api.Inrastructure.Persistence.PersistenceModels;

public class OptimisationRunPersistenceWrapper
{
    public OptimisationRunPersistence OptimisationRunPersistence { get; set; }

    public List<OptimisationResultsHourlyPersistenceWrapper> OptimisationResultsHourlyPersistence { get; set; }
}