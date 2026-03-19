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
    private IProductionUnitRepository<ElectricBoilerPersistence> _electricBoilerRepository;
    private IProductionUnitRepository<GasMotorPersistence> _gasMotorRepository; 
    
    public ProductionUnitService(IProductionUnitRepository<GasBoilerPersistence> gasBoilerRepository, IProductionUnitRepository<OilBoilerPersistence> oilBoilerRepository, IProductionUnitRepository<ElectricBoilerPersistence> electricBoilerRepository, IProductionUnitRepository<GasMotorPersistence> gasMotorRepository) 
    {
        _gasBoilerRepository = gasBoilerRepository;
        _oilBoilerRepository = oilBoilerRepository;
        _electricBoilerRepository = electricBoilerRepository;
        _gasMotorRepository = gasMotorRepository;
    }

    //TODO This should be changed to return domain model, not persistence
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
    
    //TODO This should be changed to return domain model, not persistence
    public async Task<List<OilBoilerPersistence>> GetAllOilBoilersAsync()
    {
        try
        {
            return await _oilBoilerRepository.GetAllAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in ProductionUnitService.GetAllOilBoilersAsync: {e.Message}");
            throw;
        }
    }

    public async Task<List<ElectricBoilerPersistence>> GetAllElectricBoilersAsync()
    {
        return await  _electricBoilerRepository.GetAllAsync();;
    }

    public async Task<List<GasMotorPersistence>> GetAllGasMotorsAsync()
    {
        return await  _gasMotorRepository.GetAllAsync();
    }
}