using Rdm.Api.Application.Interfaces;
using Rdm.Api.Domain.Models;
using Rdm.Api.Infrastructure.Persistence.PersistenceModels;
using Supabase;

namespace Rdm.Api.Infrastructure.Persistence;

public class OptimizationResultRepository : IOptimizationResultRepository<OptimizationResult>
{
    private readonly DatabaseContext _context;
    private readonly Client _client;

    public OptimizationResultRepository(DatabaseContext context)
    {
        _context = context;
        _client = _context.GetClient();
    }

    public async Task<List<OptimizationResult>> GetAllAsync()
    {
        var response = await _client.From<OptimizationResultPersistence>().Get();
        var models = response.Models;

        return models.Select(ToDomain).ToList();
    }

    public async Task<OptimizationResult> GetByIdAsync(Guid id)
    {
        var response = await _client.From<OptimizationResultPersistence>()
            .Where(x => x.Id == id)
            .Single();

        if (response == null) return null!;

        return ToDomain(response);
    }

    public async Task<OptimizationResult> AddAsync(OptimizationResult entity)
    {
        var persistenceModel = ToPersistence(entity);

        var response = await _client.From<OptimizationResultPersistence>()
            .Insert(persistenceModel);

        return ToDomain(response.Models.First());
    }

    public async Task UpdateAsync(OptimizationResult entity)
    {
        var persistenceModel = ToPersistence(entity);

        await _client.From<OptimizationResultPersistence>()
            .Where(x => x.Id == entity.Id)
            .Update(persistenceModel);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _client.From<OptimizationResultPersistence>()
            .Where(x => x.Id == id)
            .Delete();
    }

    private static OptimizationResult ToDomain(OptimizationResultPersistence p)
    {
        return new OptimizationResult
        {
            Id = p.Id,
            Timestamp = p.Timestamp,
            TotalCost = p.TotalCost,
            TotalCo2Emissions = p.TotalCo2Emissions
        };
    }

    private static OptimizationResultPersistence ToPersistence(OptimizationResult d)
    {
        return new OptimizationResultPersistence
        {
            Id = d.Id,
            Timestamp = d.Timestamp,
            TotalCost = d.TotalCost,
            TotalCo2Emissions = d.TotalCo2Emissions
        };
    }
}