using Am.Api.Application.Interfaces;
using Am.Api.Infrastructure.Presistence;
using Am.Api.Model.DTOs;
using Am.Api.Infrastructure.DTOs;
using Am.Api.Domain.Models;

namespace Am.Api.Application.Services;

/// <summary>
/// In charge of handling all operations in relation to a production unit. Maps the functionality to specific use cases.
/// The class stores methods about retrieval of different production units.
/// </summary>
public class ProductionUnitService: IProductionUnitService
{   
    private IProductionUnitRepository<GasBoiler> _gasBoilerRepository;
    private IProductionUnitRepository<OilBoiler> _oilBoilerRepository;
    private IProductionUnitRepository<ElectricBoiler> _electricBoilerRepository;
    private IProductionUnitRepository<GasMotor> _gasMotorRepository; 
    private IMaintenanceRepository _maintenanceRepository;
    
    public ProductionUnitService(
        IProductionUnitRepository<GasBoiler> gasBoilerRepository,
        IProductionUnitRepository<OilBoiler> oilBoilerRepository,
        IProductionUnitRepository<ElectricBoiler> electricBoilerRepository,
        IProductionUnitRepository<GasMotor> gasMotorRepository,
        IMaintenanceRepository maintenanceRepository)
    {
        _gasBoilerRepository = gasBoilerRepository;
        _oilBoilerRepository = oilBoilerRepository;
        _electricBoilerRepository = electricBoilerRepository;
        _gasMotorRepository = gasMotorRepository;
        _maintenanceRepository = maintenanceRepository;
    }

    public async Task<List<GasBoiler>> GetAllGasBoilersAsync()
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

    public async Task<List<OilBoiler>> GetAllOilBoilersAsync()
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

    public async Task<List<ElectricBoiler>> GetAllElectricBoilersAsync()
    {
        return await  _electricBoilerRepository.GetAllAsync();
    }

    public async Task<List<GasMotor>> GetAllGasMotorsAsync()
    {
        return await  _gasMotorRepository.GetAllAsync();
    }

    public async Task<ProductionUnitMaintenance> GetProductionUnitMaintenanceByIdAsync(int Id)
    {
        try
        {
            ProductionUnitMaintenance? maintenance = (await _maintenanceRepository.GetAllProductionUnitMaintenanceAsync())
                .FirstOrDefault(p => p.Id == Id);

            if (maintenance is null)
            {
                throw new KeyNotFoundException($"No ProductionUnitMaintenance found with Id {Id}.");
            }

            return maintenance;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in ProductionUnitService.GetProductionUnitMaintenanceByIdAsync: {e.Message}");
            throw;
        }
    }
}