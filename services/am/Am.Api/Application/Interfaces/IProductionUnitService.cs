using Am.Api.Model.DTOs;

namespace Am.Api.Application.Interfaces;

public interface IProductionUnitService
{
   Task<List<GasBoilerPersistence>> GetAllGasBoilersAsync();
   Task <List<OilBoilerPersistence>> GetAllOilBoilersAsync();
   Task<List<ElectricBoilerPersistence>> GetAllElectricBoilersAsync();
   Task<List<GasMotorPersistence>> GetAllGasMotorsAsync();
   //TODO More to be added
}