using Am.Api.Application.Interfaces;
using Am.Api.Model.DTOs;
using Am.Api.Domain.Models;
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
                productionUnitMaintenances.Add( new ProductionUnitMaintenance
                {
                    Id = p.Id,
                    UnitTypeId = p.UnitTypeId,
                    UnitId = p.UnitId,
                    CreatedAt = p.CreatedAt,
                    FromDate = p.FromDate,
                    ToDate = p.ToDate,
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
}
