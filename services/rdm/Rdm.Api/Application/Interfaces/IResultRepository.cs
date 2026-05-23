namespace Rdm.Api.Application.Interfaces;

using Rdm.Api.Inrastructure.Persistence.PersistenceModels;

public interface IResultRepository
{
    Task<List<OptimisationRunWithHourlyResultsPersistence>> GetAllOptimisationResults();

    Task<OptimisationRunPersistence> GetLatestOptimisationResult();

    Task<bool> SaveOptimisationResult(OptimisationRunPersistenceWrapper result);
}