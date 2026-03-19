using Am.Api.Application.Interfaces;
using Am.Api.Infrastructure.Presistence;
using Am.Api.Model.DTOs;

namespace Am.Api.Application.Services;

/// <summary>
/// In charge of handling all operations in relation to a production unit. Maps the functionality to specific use cases.
/// The class stores methods about retrieval of different production units
/// </summary>
public class ProductionUnitService: IProductionUnitService
{   
    private IProductionUnitRepository<GasBoilerPersistence> _gasBoilerRepository;
    private IProductionUnitRepository<OilBoilerPersistence> _oilBoilerRepository;
    
    public ProductionUnitService(IProductionUnitRepository<GasBoilerPersistence> gasBoilerRepository, IProductionUnitRepository<OilBoilerPersistence> oilBoilerRepository) 
    {
        _gasBoilerRepository = gasBoilerRepository;
        _oilBoilerRepository = oilBoilerRepository;
    }

    public async Task<List<GasBoilerPersistence>> GetAllGasBoilersAsync()
    {
        try
        {
            return await _gasBoilerRepository.GetAllAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in ProductionUnitService.GetAllGasBoilersAsync: {e.Message}");
            throw; 
        }
    }
}