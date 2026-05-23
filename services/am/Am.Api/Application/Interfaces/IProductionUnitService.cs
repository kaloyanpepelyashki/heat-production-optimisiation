namespace Am.Api.Application.Interfaces;

using Am.Api.Domain.Models;
using Am.Api.Model.DTOs;

public interface IProductionUnitService
{
   Task<List<GasBoiler>> GetAllGasBoilersAsync();

   Task <List<OilBoiler>> GetAllOilBoilersAsync();

   Task<List<ElectricBoiler>> GetAllElectricBoilersAsync();

   Task<List<GasMotor>> GetAllGasMotorsAsync();

   Task<ProductionUnitMaintenance> GetProductionUnitMaintenanceByIdAsync(int Id);

   Task<int> PostProductionUnitMaintenanceAsync(ProductionUnitMaintenance productionUnitMaintenance);
}