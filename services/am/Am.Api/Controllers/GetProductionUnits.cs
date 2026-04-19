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
    //TODO - A more in depth error handling must be implemented
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
    
    //TODO - A more in depth error handling must be implemented here (different types of exceptions coming from the layers below must be handled differently)
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
    //TODO - A more in depth error handling must be implemented here (different types of exceptions coming from the layers below must be handled differently)
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

    [HttpGet("lowestProductionCostBoiler")]
    public async Task<IActionResult> GetLowestProductionCostBoiler()
    {
        try
        {
            List<GasBoiler> gasBoilersResult = await _productionUnitService.GetAllGasBoilersAsync();
            List<OilBoiler> oilBoilerResult = await _productionUnitService.GetAllOilBoilersAsync();

            List<GasBoilerDTO> gasBoilersDTOs = gasBoilersResult.Select(x => new GasBoilerDTO
            {
                Id = x.Id,
                Name = x.Name,
                MaxHeat = x.MaxHeat,
                ProductionCost = x.ProductionCost,
                Co2Emissions = x.Co2Emissions,
                GasConsumption = x.GasConsumption,
            }).ToList();

            List<OilBoilerDTO> oilBoilerDtos = oilBoilerResult.Select(obj => new OilBoilerDTO
            {
                Id = obj.Id,
                Name = obj.Name,
                MaxHeat = obj.MaxHeat,
                ProductionCost = obj.ProductionCost,
                Co2Emissions = obj.Co2Emissions,
                OilConsumption = obj.OilConsumption,
            }).ToList();

            List<IProductionCostDTO> boilers = gasBoilersDTOs.Cast<IProductionCostDTO>().ToList();
            boilers.AddRange(oilBoilerDtos.Cast<IProductionCostDTO>());

            IProductionCostDTO? lowestProductionCostBoiler = null;
            foreach (IProductionCostDTO boiler in boilers)
            {
                if (lowestProductionCostBoiler == null || boiler.ProductionCost < lowestProductionCostBoiler.ProductionCost)
                {
                    lowestProductionCostBoiler = boiler;
                }
            }

            return Ok(lowestProductionCostBoiler);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception in Controller/lowestProductionCostBoiler, {e.Message}, {e.GetType()}");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("lowestConsumptionBoiler")]
    public async Task<IActionResult> GetLowestConsumptionBoiler()
    {
        try
        {
            List<GasBoiler> gasBoilersResult = await _productionUnitService.GetAllGasBoilersAsync();
            List<OilBoiler> oilBoilerResult = await _productionUnitService.GetAllOilBoilersAsync();

            List<GasBoilerDTO> gasBoilersDTOs = gasBoilersResult.Select(x => new GasBoilerDTO
            {
                Id = x.Id,
                Name = x.Name,
                MaxHeat = x.MaxHeat,
                ProductionCost = x.ProductionCost,
                Co2Emissions = x.Co2Emissions,
                GasConsumption = x.GasConsumption,
            }).ToList();

            List<OilBoilerDTO> oilBoilerDtos = oilBoilerResult.Select(obj => new OilBoilerDTO
            {
                Id = obj.Id,
                Name = obj.Name,
                MaxHeat = obj.MaxHeat,
                ProductionCost = obj.ProductionCost,
                Co2Emissions = obj.Co2Emissions,
                OilConsumption = obj.OilConsumption,
            }).ToList();

            List<IConsumptionDTO> boilers = gasBoilersDTOs.Cast<IConsumptionDTO>().ToList();
            boilers.AddRange(oilBoilerDtos.Cast<IConsumptionDTO>());

            IConsumptionDTO? lowestConsumptionBoiler = null;
            foreach (IConsumptionDTO boiler in boilers)
            {
                if (lowestConsumptionBoiler == null || boiler.Consumption < lowestConsumptionBoiler.Consumption)
                {
                    lowestConsumptionBoiler = boiler;
                }
            }

            return Ok(lowestConsumptionBoiler);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception in Controller/lowestConsumptionBoiler, {e.Message}, {e.GetType()}");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("allActiveGasBoilers")]
    public async Task<IActionResult> GetAllActiveGasBoilers([FromQuery] DateTime startPeriod)
    {
        try
        {
            List<GasBoiler> gasBoilersResult = await _productionUnitService.GetActiveGasBoilersAsync(startPeriod);

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
            Console.WriteLine($"Exception in Controller/allActiveGasBoilers, {e.Message}, {e.GetType()}");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("allActiveOilBoilers")]
    public async Task<IActionResult> GetAllActiveOilBoilers([FromQuery] DateTime startPeriod)
    {
        try
        {
            List<OilBoiler> oilBoilerResult = await _productionUnitService.GetActiveOilBoilersAsync(startPeriod);

            List<OilBoilerDTO> oilBoilerDtos = oilBoilerResult.Select(obj => new OilBoilerDTO
            {
                Id = obj.Id,
                Name = obj.Name,
                MaxHeat = obj.MaxHeat,
                ProductionCost = obj.ProductionCost,
                Co2Emissions = obj.Co2Emissions,
                OilConsumption = obj.OilConsumption,
            }).ToList();

            return Ok(oilBoilerDtos);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception in Controller/allActiveOilBoilers, {e.Message}, {e.GetType()}");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("allActiveElectricBoilers")]
    public async Task<IActionResult> GetAllActiveElectricBoilers([FromQuery] DateTime startPeriod)
    {
        try
        {
            List<ElectricBoiler> electricBoilersResult = await _productionUnitService.GetActiveElectricBoilersAsync(startPeriod);

            List<ElectricBoilerDTO> boilers = electricBoilersResult.Select(elBoiler => new ElectricBoilerDTO
            {
                Id = elBoiler.Id,
                Name = elBoiler.Name,
                MaxHeat = elBoiler.MaxHeat,
                ProductionCost = elBoiler.ProductionCost,
                MaxElectricity = elBoiler.MaxElectricity,
            }).ToList();

            return Ok(boilers);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception in Controller/allActiveElectricBoilers, {e.Message}, {e.GetType()}");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("allActiveGasMotors")]
    public async Task<IActionResult> GetAllActiveGasMotors([FromQuery] DateTime startPeriod)
    {
        try
        {
            List<GasMotor> gasMotorResult = await _productionUnitService.GetActiveGasMotorsAsync(startPeriod);

            List<GasMotorDTO> motors = gasMotorResult.Select(gasMotor => new GasMotorDTO
            {
                Id = gasMotor.Id,
                Name = gasMotor.Name,
                MaxHeat = gasMotor.MaxHeat,
                ProductionCost = gasMotor.ProductionCost,
                MaxElectricity = gasMotor.MaxElectricity,
                Co2Emissions = gasMotor.Co2Emissions,
                GasConsumption = gasMotor.GasConsumption
            }).ToList();

            return Ok(motors);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception in Controller/allActiveGasMotors, {e.Message}, {e.GetType()}");
            return StatusCode(500, "Internal Server Error");
        }
    }
}