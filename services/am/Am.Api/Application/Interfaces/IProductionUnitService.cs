using Am.Api.Model.DTOs;

namespace Am.Api.Application.Interfaces;

public interface IProductionUnitService
{
   Task<List<GasBoilerPersistence>> GetAllGasBoilersAsync();
   //TODO More to be added
}