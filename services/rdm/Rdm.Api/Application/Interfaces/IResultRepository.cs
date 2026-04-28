using Rdm.Api.Inrastructure.Persistence.PersistenceModels;

namespace Rdm.Api.Application.Interfaces;

public interface IResultRepository
{
    Task<List<OptimisationRunPersistence>> GetAllOptimisationResults();
    Task<OptimisationRunPersistence> GetLatestOptimisationResult();
    Task<bool> SaveOptimisationResult(OptimisationRunPersistence result);
}