using Rdm.Api.Application.Model;

namespace Rdm.Api.Application.Interfaces;

public interface IOptimisationResultService
{
    public Task<List<OptimisationRun>> GetAllOptimisationResults();
    // public Task<OptimisationRun> GetLatestOptimisationResult();
}   