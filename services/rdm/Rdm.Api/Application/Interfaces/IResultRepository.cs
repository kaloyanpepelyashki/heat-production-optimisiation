using Rdm.Api.Inrastructure.Persistence.PersistenceModels;

namespace Rdm.Api.Application.Interfaces;

public interface IResultRepository
{
    Task<List<ResultPersistence>> GetAllOptimisationResults();
    Task<ResultPersistence> GetLatestOptimisationResult();
    Task<ResultPersistence> SaveOptimisationResult(ResultPersistence result);
}