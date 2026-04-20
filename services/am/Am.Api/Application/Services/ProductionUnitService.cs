using Am.Api.Application.Interfaces;
using Am.Api.Infrastructure.Presistence;
using Am.Api.Model.DTOs;
using Am.Api.Domain.Models;

namespace Am.Api.Application.Services;

public class ProductionUnitService: IProductionUnitService
{   
    private IProductionUnitRepository<GasBoiler> _gasBoilerRepository;
    private IProductionUnitRepository<OilBoiler> _oilBoilerRepository;
    private IProductionUnitRepository<ElectricBoiler> _electricBoilerRepository;
    private IProductionUnitRepository<GasMotor> _gasMotorRepository; 
    
    public ProductionUnitService(IProductionUnitRepository<GasBoiler> gasBoilerRepository, IProductionUnitRepository<OilBoiler> oilBoilerRepository, IProductionUnitRepository<ElectricBoiler> electricBoilerRepository, IProductionUnitRepository<GasMotor> gasMotorRepository) 
    {
        _gasBoilerRepository = gasBoilerRepository;
        _oilBoilerRepository = oilBoilerRepository;
        _electricBoilerRepository = electricBoilerRepository;
        _gasMotorRepository = gasMotorRepository;
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
}