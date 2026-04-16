namespace Rdm.Api.Application.Interfaces;

using Rdm.Api.Domain.Models;

public interface IOptimizationResultService
{
    Task<List<OptimizationResult>> GetAllResultsAsync();

    Task<List<OptimizationResult>> GetResultsByPeriodAsync(string period);

    Task<OptimizationResult> CreateResultAsync(OptimizationResult result);

    Task<OptimizationResult?> UpdateResultAsync(int id, OptimizationResult result);
}

