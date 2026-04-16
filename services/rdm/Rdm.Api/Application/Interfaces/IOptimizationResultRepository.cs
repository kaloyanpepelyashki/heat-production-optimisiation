namespace Rdm.Api.Application.Interfaces;

using Rdm.Api.Domain.Models;

public interface IOptimizationResultRepository
{
    Task<List<OptimizationResult>> GetAllAsync();

    Task<List<OptimizationResult>> GetByPeriodAsync(string period);

    Task<OptimizationResult> CreateAsync(OptimizationResult result);
}

