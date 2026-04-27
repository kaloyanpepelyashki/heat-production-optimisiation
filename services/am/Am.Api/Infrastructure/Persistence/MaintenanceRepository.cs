using Am.Api.Application.Interfaces;
using Am.Api.Domain.Models;
using Am.Api.Model.DTOs;
using Supabase;
using Supabase.Postgrest.Responses;

namespace Am.Api.Infrastructure.Presistence;

public sealed class MaintenanceRepository : IMaintenanceRepository
{
    private readonly Client _client;
    private readonly ILogger<MaintenanceRepository> _logger;

    public MaintenanceRepository(DatabaseContext context, ILogger<MaintenanceRepository> logger)
    {
        _client = context.GetClient();
        _logger = logger;
    }

    public async Task<List<ProductionUnitMaintenance>> GetAllProductionUnitMaintenanceAsync()
    {
        try
        {
            ModeledResponse<ProductionUnitMaintenancePersistence> result = await _client
                .From<ProductionUnitMaintenancePersistence>()
                .Get();

            List<ProductionUnitMaintenancePersistence> persistances = result.Models;

            List<ProductionUnitMaintenance> productionUnitMaintenances = new List<ProductionUnitMaintenance>();

            foreach (ProductionUnitMaintenancePersistence p in persistances)
            {
                productionUnitMaintenances.Add(new ProductionUnitMaintenance
                {
                    Id = p.Id,
                    UnitTypeId = p.UnitTypeId,
                    UnitId = p.UnitId,
                    CreatedAt = p.CreatedAt,
                    FromDate = p.FromDate,
                    ToDate = p.ToDate,
                    PeriodId = p.PeriodId,
                    ScenarioId = p.ScenarioId,
                });
            }
            return productionUnitMaintenances;
        }
        catch (Exception e)
        {
            _logger.LogError($"Error fetching all in MaintenanceRepository {e.GetType()} {e.Message}");
            throw;
        }
    }

    public async Task<int> PostProductionUnitMaintenanceAsync(ProductionUnitMaintenance maintenance)
    {
        try
        {
            if (maintenance is null)
            {
                throw new ArgumentNullException(nameof(maintenance));
            }

            ProductionUnitMaintenancePersistence persistence = new ProductionUnitMaintenancePersistence
            {
                UnitTypeId = maintenance.UnitTypeId,
                UnitId = maintenance.UnitId,
                CreatedAt = maintenance.CreatedAt == default ? DateTime.UtcNow : maintenance.CreatedAt,
                FromDate = maintenance.FromDate,
                ToDate = maintenance.ToDate,
                PeriodId = maintenance.PeriodId,
                ScenarioId = maintenance.ScenarioId,
            };

            ModeledResponse<ProductionUnitMaintenancePersistence> result = await _client
                .From<ProductionUnitMaintenancePersistence>()
                .Insert(persistence);

            return result.Model.Id;
        }
        catch (Exception e)
        {
            _logger.LogError($"Error inserting ProductionUnitMaintenance in MaintenanceRepository {e.GetType()} {e.Message}");
            throw;
        }
    }
}
