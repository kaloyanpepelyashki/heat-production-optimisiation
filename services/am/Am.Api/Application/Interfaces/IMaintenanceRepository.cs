using Am.Api.Domain.Models;
namespace Am.Api.Application.Interfaces;

public interface IMaintenanceRepository
{
    Task<List<ProductionUnitMaintenance>> GetAllProductionUnitMaintenanceAsync();
}
