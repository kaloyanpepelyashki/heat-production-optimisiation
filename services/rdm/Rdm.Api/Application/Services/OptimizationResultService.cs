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
        ValidateResult(result);
        return await _repository.CreateAsync(result);
    }

    public async Task<OptimizationResult?> UpdateResultAsync(int id, OptimizationResult result)
    {
        ValidateResult(result);
        return await _repository.UpdateAsync(id, result);
    }

    private void ValidateResult(OptimizationResult result)
    {
        if (result == null)
            throw new ArgumentNullException(nameof(result));

        if (string.IsNullOrWhiteSpace(result.ProductionUnit))
            throw new ArgumentException("ProductionUnit cannot be null or empty.", nameof(result.ProductionUnit));

        if (result.TotalHeat < 0 || result.TotalCost < 0 || result.TotalEmissions < 0)
            throw new ArgumentException("Values cannot be negative.");
    }
}