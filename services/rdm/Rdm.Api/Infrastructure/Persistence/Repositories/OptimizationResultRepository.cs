namespace Rdm.Api.Infrastructure.Persistence.Repositories;

using Rdm.Api.Application.Interfaces;
using Rdm.Api.Domain.Models;
using Supabase;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

public class OptimizationResultRepository : IOptimizationResultRepository
{
    private readonly Client _client;

    public OptimizationResultRepository(Client supabaseClient)
    {
        _client = supabaseClient;
    }

    public async Task<List<OptimizationResult>> GetAllAsync()
    {
        var response = await _client.From<OptimizationResultData>().Get();
        return response.Models.Select(ToDomain).ToList();
    }

    public async Task<List<OptimizationResult>> GetByPeriodAsync(string period)
    {
        var response = await _client.From<OptimizationResultData>()
            .Where(r => r.ProductionUnit.Contains(period))
            .Get();
        return response.Models.Select(ToDomain).ToList();
    }

    public async Task<OptimizationResult> CreateAsync(OptimizationResult result)
    {
        var data = new OptimizationResultData
        {
            ProductionUnit = result.ProductionUnit,
            TotalHeat = result.TotalHeat,
            TotalCost = result.TotalCost,
            TotalEmissions = result.TotalEmissions,
            CreatedAt = DateTime.UtcNow
        };

        var response = await _client.From<OptimizationResultData>().Insert(data);
        return ToDomain(response.Models.First());
    }

    public async Task<OptimizationResult?> UpdateAsync(int id, OptimizationResult result)
    {
        var data = new OptimizationResultData
        {
            ProductionUnit = result.ProductionUnit,
            TotalHeat = result.TotalHeat,
            TotalCost = result.TotalCost,
            TotalEmissions = result.TotalEmissions
        };

        var response = await _client.From<OptimizationResultData>()
            .Where(r => r.Id == id)
            .Update(data);
        return response.Models.Count > 0 ? ToDomain(response.Models.First()) : null;
    }

    private static OptimizationResult ToDomain(OptimizationResultData data)
    {
        return new OptimizationResult
        {
            Id = data.Id,
            ProductionUnit = data.ProductionUnit,
            TotalHeat = data.TotalHeat,
            TotalCost = data.TotalCost,
            TotalEmissions = data.TotalEmissions,
            CreatedAt = data.CreatedAt
        };
    }
}

[Table("result_data")]
public class OptimizationResultData : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("production_unit")]
    public string ProductionUnit { get; set; } = string.Empty;

    [Column("total_heat")]
    public float TotalHeat { get; set; }

    [Column("total_cost")]
    public float TotalCost { get; set; }

    [Column("total_emissions")]
    public float TotalEmissions { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}