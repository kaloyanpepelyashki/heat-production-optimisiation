using Am.Api.Application.Interfaces;
using Am.Api.Infrastructure.DTOs;
using Am.Api.Model.DTOs;
using Microsoft.AspNetCore.Mvc;
using Am.Api.Domain.Models;

namespace Am.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GetProductionUnits : Controller
{   
    private readonly IProductionUnitService _productionUnitService;
    
    public GetProductionUnits(IProductionUnitService productionUnitService)
    {
        _productionUnitService = productionUnitService;
    }
    
    [HttpGet("allGasBoilers")]
    public async Task<IActionResult> GetAllGasBoilers()
    {
        try
        {
            List<GasBoiler> gasBoilersResult = await _productionUnitService.GetAllGasBoilersAsync();
            
            List<GasBoilerDTO> gasBoilersDTOs = gasBoilersResult.Select(x => new GasBoilerDTO
            {
                Id = x.Id,
                Name = x.Name,
                MaxHeat = x.MaxHeat,
                ProductionCost = x.ProductionCost,
                Co2Emissions = x.Co2Emissions,
                GasConsumption = x.GasConsumption,
            }).ToList();
            
            return Ok(gasBoilersDTOs);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception in Controller/allGasBoilers, {e.Message}, {e.GetType()}");
            return StatusCode(500, "Internal Server Error");
        }
    }
    
    [HttpGet("allOilBoilers")]
    public async Task<IActionResult> GetAllOilBoilers()
    {
        try
        {
            List<OilBoiler> oilBoilerResult = await _productionUnitService.GetAllOilBoilersAsync();
            
            List<OilBoilerDTO> oilBoilerDtos = oilBoilerResult.Select(obj => new OilBoilerDTO
            {
                Id = obj.Id,
                Name = obj.Name,
                MaxHeat = obj.MaxHeat,
                ProductionCost = obj.ProductionCost,
                Co2Emissions = obj.Co2Emissions,
                OilConsumption = obj.OilConsumption
            }).ToList();
            
            return Ok(oilBoilerDtos);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception in Controller/allOilBoilers, {e.Message}, {e.GetType()}");
            return StatusCode(500, "Internal Server Error");
        }
    }
    
    [HttpGet("allElectricBoilers")]
    public async Task<IActionResult> GetAllElectricBoilers()
    {
        try
        {
            List<ElectricBoiler> electricBoilersResult =
                await _productionUnitService.GetAllElectricBoilersAsync();

            List<ElectricBoilerDTO> electricBoilerDtos = electricBoilersResult.Select(elBoiler =>
                new ElectricBoilerDTO
                {
                    Id = elBoiler.Id,
                    Name = elBoiler.Name,
                    MaxHeat = elBoiler.MaxHeat,
                    ProductionCost = elBoiler.ProductionCost,
                    MaxElectricity = elBoiler.MaxElectricity,
                }).ToList();

            return Ok(electricBoilerDtos);
        }
        catch (Exception e)
        {   
            Console.WriteLine($"Exception in Controller/allElectricBoilers, {e.Message}, {e.GetType()}");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("allGasMotors")]
    public async Task<IActionResult> GetAllGasMotors()
    {
        try
        {
            List<GasMotor> gasMotorResult = await _productionUnitService.GetAllGasMotorsAsync();

            List<GasMotorDTO> gasMotorDtos = gasMotorResult.Select(gasMotor => new GasMotorDTO
            {
                Id = gasMotor.Id,
                Name = gasMotor.Name,
                MaxHeat = gasMotor.MaxHeat,
                ProductionCost = gasMotor.ProductionCost,
                MaxElectricity = gasMotor.MaxElectricity,
                Co2Emissions = gasMotor.Co2Emissions,
                GasConsumption = gasMotor.GasConsumption
            }).ToList();

            return Ok(gasMotorDtos);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception in Controller/allGasMotors, {e.Message}, {e.GetType()}");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("productionUnitMaintenancesById/{id:int}")]
    public async Task<IActionResult> GetProductionUnitMaintenanceById(int id)
    {
        try
        {
            ProductionUnitMaintenance result =
                await _productionUnitService.GetProductionUnitMaintenanceByIdAsync(id);

            ProductionUnitMaintenanceDTO productionUnitMaintenanceDto = new ProductionUnitMaintenanceDTO
            {
                Id = result.Id,
                UnitType = result.UnitType,
                UnitId = result.UnitId,
                CreatedAt = result.CreatedAt,
                FromDate = result.FromDate,
                ToDate = result.ToDate,
                PeriodId = result.PeriodId,
                ScenarioId = result.ScenarioId,
            };

            return Ok(productionUnitMaintenanceDto);
        }
        catch (KeyNotFoundException e)
        {
            Console.WriteLine($"Exception in Controller/productionUnitMaintenancesById, {e.Message}, {e.GetType()}");
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception in Controller/productionUnitMaintenancesById, {e.Message}, {e.GetType()}");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPost("productionUnitMaintenance")]
    [Consumes("application/json")]
    public async Task<IActionResult> PostProductionUnitMaintenance([FromBody] ProductionUnitMaintenanceDTO productionUnitMaintenance)
    {
        try
        {
            ProductionUnitMaintenance maintenance = new ProductionUnitMaintenance
            {
                UnitType = productionUnitMaintenance.UnitType,
                UnitId = productionUnitMaintenance.UnitId,
                CreatedAt = productionUnitMaintenance.CreatedAt,
                FromDate = productionUnitMaintenance.FromDate,
                ToDate = productionUnitMaintenance.ToDate,
                PeriodId = productionUnitMaintenance.PeriodId,
                ScenarioId = productionUnitMaintenance.ScenarioId,
            };

            int result =
                await _productionUnitService.PostProductionUnitMaintenanceAsync(maintenance);

            return Ok(result);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception in Controller/productionUnitMaintenance, {e.Message}, {e.GetType()}");
            return StatusCode(500, "Internal Server Error");
        }
    }
}