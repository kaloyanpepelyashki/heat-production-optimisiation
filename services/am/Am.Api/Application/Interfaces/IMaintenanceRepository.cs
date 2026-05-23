namespace Am.Api.Application.Interfaces;

using Am.Api.Domain.Models;

public interface IMaintenanceRepository
{
    Task<List<ProductionUnitMaintenance>> GetAllProductionUnitMaintenanceAsync();

    Task<int> PostProductionUnitMaintenanceAsync(ProductionUnitMaintenance maintenance);
}
