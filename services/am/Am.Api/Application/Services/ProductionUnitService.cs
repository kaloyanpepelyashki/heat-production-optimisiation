using Am.Api.Application.Interfaces;
using Am.Api.Infrastructure.Presistence;
using Am.Api.Model.DTOs;
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

    public async Task<List<GasBoiler>> GetActiveGasBoilersAsync(DateTime startPeriod)
    {
        List<GasBoiler> units = await GetAllGasBoilersAsync();
        return await FilterActiveAtAsync(units, startPeriod);
    }

    public async Task<List<OilBoiler>> GetActiveOilBoilersAsync(DateTime startPeriod)
    {
        List<OilBoiler> units = await GetAllOilBoilersAsync();
        return await FilterActiveAtAsync(units, startPeriod);
    }

    public async Task<List<ElectricBoiler>> GetActiveElectricBoilersAsync(DateTime startPeriod)
    {
        List<ElectricBoiler> units = await GetAllElectricBoilersAsync();
        return await FilterActiveAtAsync(units, startPeriod);
    }

    public async Task<List<GasMotor>> GetActiveGasMotorsAsync(DateTime startPeriod)
    {
        List<GasMotor> units = await GetAllGasMotorsAsync();
        return await FilterActiveAtAsync(units, startPeriod);
    }

    private async Task<List<T>> FilterActiveAtAsync<T>(List<T> units, DateTime startPeriod) where T : ProductionUnit
    {
        int? inactiveUnitId = await GetInactiveUnitIdAtAsync(startPeriod);
        if (inactiveUnitId is null)
        {
            return units;
        }

        return units.Where(u => u.Id != inactiveUnitId.Value).ToList();
    }

    private async Task<int?> GetInactiveUnitIdAtAsync(DateTime startPeriod)
    {
        // Finds the coresponding period and maintenance period.
        List<MaintenancePeriodPersistence> allMaintenancePeriods = await _maintenanceRepository.GetAllMaintenancePeriodsAsync();

        MaintenancePeriodPersistence? overlappingMaintenancePeriod = allMaintenancePeriods
            .FirstOrDefault(mp => mp.StartTime <= startPeriod && mp.EndTime > startPeriod);

        if (overlappingMaintenancePeriod is null)
        {
            return null;
        }

        List<ProductionUnitMaintenancePersistence> allLinks = await _maintenanceRepository.GetAllProductionUnitMaintenanceAsync();

        return allLinks
            .FirstOrDefault(link => link.MaintenancePeriodId == overlappingMaintenancePeriod.Id)
            ?.UnitId;
    }
}