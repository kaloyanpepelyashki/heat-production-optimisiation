using Am.Api.Model.DTOs;

namespace Am.Api.Application.Interfaces;

public interface IMaintenanceRepository
{
    Task<List<MaintenancePeriodPersistence>> GetAllMaintenancePeriodsAsync();
    Task<List<ProductionUnitMaintenancePersistence>> GetAllProductionUnitMaintenanceAsync();
}
