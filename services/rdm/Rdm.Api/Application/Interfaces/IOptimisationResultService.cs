using Rdm.Api.Application.Model;

namespace Rdm.Api.Application.Interfaces;

public interface IOptimisationResultService
{
    public Task<List<OptimisationResult>> GetAllOptimisationResults();
    public Task<OptimisationResult> GetLatestOptimisationResult();
}   