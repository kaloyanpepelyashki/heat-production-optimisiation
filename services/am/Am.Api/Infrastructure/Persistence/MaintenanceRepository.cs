using Am.Api.Application.Interfaces;
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

    public async Task<List<MaintenancePeriodPersistence>> GetAllMaintenancePeriodsAsync()
    {
        try
        {
            ModeledResponse<MaintenancePeriodPersistence> result = await _client
                .From<MaintenancePeriodPersistence>()
                .Get();

            return result.Models;
        }
        catch (Exception e)
        {
            _logger.LogError($"Error fetching all in MaintenanceRepository {e.GetType()} {e.Message}");
            throw;
        }
    }

    public async Task<List<ProductionUnitMaintenancePersistence>> GetAllProductionUnitMaintenanceAsync()
    {
        try
        {
            ModeledResponse<ProductionUnitMaintenancePersistence> result = await _client
                .From<ProductionUnitMaintenancePersistence>()
                .Get();

            return result.Models;
        }
        catch (Exception e)
        {
            _logger.LogError($"Error fetching all in MaintenanceRepository {e.GetType()} {e.Message}");
            throw;
        }
    }
}
