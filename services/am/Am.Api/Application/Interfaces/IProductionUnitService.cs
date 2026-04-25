using Am.Api.Model.DTOs;
using Am.Api.Domain.Models;

namespace Am.Api.Application.Interfaces;

public interface IProductionUnitService
{
   Task<List<GasBoiler>> GetAllGasBoilersAsync();
   Task <List<OilBoiler>> GetAllOilBoilersAsync();
   Task<List<ElectricBoiler>> GetAllElectricBoilersAsync();
   Task<List<GasMotor>> GetAllGasMotorsAsync();
   Task<ProductionUnitMaintenance> GetProductionUnitMaintenanceByIdAsync(int Id);
   //TODO More to be added
}