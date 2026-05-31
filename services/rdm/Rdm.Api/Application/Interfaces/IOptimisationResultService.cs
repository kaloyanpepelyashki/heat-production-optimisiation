namespace Rdm.Api.Application.Interfaces;

using Rdm.Api.Application.Model;

public interface IOptimisationResultService
{
    public Task<List<OptimisationRun>> GetAllOptimisationResults();

    Task<bool> SaveOptimisationRun(OptimisationRun optimisationRun);
}