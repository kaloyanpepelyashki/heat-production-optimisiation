using Am.Api.Model.DTOs;
using Am.Api.Domain.Models;

namespace Am.Api.Application.Interfaces;

public interface IProductionUnitService
{
   Task<List<GasBoiler>> GetAllGasBoilersAsync();
   Task <List<OilBoiler>> GetAllOilBoilersAsync();
   Task<List<ElectricBoiler>> GetAllElectricBoilersAsync();
   Task<List<GasMotor>> GetAllGasMotorsAsync();

   Task<List<GasBoiler>> GetActiveGasBoilersAsync(DateTime startPeriod);
   Task<List<OilBoiler>> GetActiveOilBoilersAsync(DateTime startPeriod);
   Task<List<ElectricBoiler>> GetActiveElectricBoilersAsync(DateTime startPeriod);
   Task<List<GasMotor>> GetActiveGasMotorsAsync(DateTime startPeriod);
   //TODO More to be added
}