namespace Rdm.Api.Application.Services;

using Rdm.Api.Application.Interfaces;
using Rdm.Api.Domain.Models;

public class OptimizationResultService : IOptimizationResultService
{
    private readonly IOptimizationResultRepository _repository;

    public OptimizationResultService(IOptimizationResultRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<OptimizationResult>> GetAllResultsAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<List<OptimizationResult>> GetResultsByPeriodAsync(string period)
    {
        return await _repository.GetByPeriodAsync(period);
    }

    public async Task<OptimizationResult> CreateResultAsync(OptimizationResult result)
    {
        return await _repository.CreateAsync(result);
    }
}