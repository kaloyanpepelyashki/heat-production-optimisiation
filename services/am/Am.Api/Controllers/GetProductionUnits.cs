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

    [HttpGet("lowestProdictionCostBoiler")]
    public async Task<IActionResult> GetLowestProdictionCostBoiler()
    {
        // gets all of the boilers
        var actionResultGasBoilers = await GetAllGasBoilers();
        var actionResultOilBoilers = await GetAllOilBoilers();

        if (actionResultGasBoilers is OkObjectResult && actionResultOilBoilers is OkObjectResult)
        {
            OkObjectResult resultGasBoilers = (OkObjectResult)actionResultGasBoilers;
            OkObjectResult resultOilBoilers = (OkObjectResult)actionResultOilBoilers;

            List<IProductionCostDTO> boilers = (List<IProductionCostDTO>)resultGasBoilers.Value;       
            boilers.AddRange((List<IProductionCostDTO>)resultOilBoilers.Value); // combining both lists

            IProductionCostDTO lowestProductionCostBoiler = null;

            // Finding the gas boiler with the lowest production cost
            foreach (IProductionCostDTO boiler in boilers)
            {
                if (lowestProductionCostBoiler == null)
                {
                    lowestProductionCostBoiler = boiler;
                }

                if (boiler.ProductionCost < lowestProductionCostBoiler.ProductionCost)
                {
                    lowestProductionCostBoiler = boiler;
                }
            }
            return Ok(lowestProductionCostBoiler);
        }
        else
        {
            Console.WriteLine($"Exception in Controller/lowestProdictionCostBoiler");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("lowestConsumptionCostBoiler")]
    public async Task<IActionResult> GetLowestConsumptionBoiler()
    {
        // gets all of the boilers
        var actionResultGasBoilers = await GetAllGasBoilers();
        var actionResultOilBoilers = await GetAllOilBoilers();

        if (actionResultGasBoilers is OkObjectResult && actionResultOilBoilers is OkObjectResult)
        {
            OkObjectResult resultGasBoilers = (OkObjectResult)actionResultGasBoilers;
            OkObjectResult resultOilBoilers = (OkObjectResult)actionResultOilBoilers;

            List<IConsumptionDTO> boilers = (List<IConsumptionDTO>)resultGasBoilers.Value;       
            boilers.AddRange((List<IConsumptionDTO>)resultOilBoilers.Value); // combining both lists

            IConsumptionDTO lowestConsumptionBoiler = null;

            // Finding the gas boiler with the lowest production cost
            foreach (IConsumptionDTO boiler in boilers)
            {
                if (lowestConsumptionBoiler == null)
                {
                    lowestConsumptionBoiler = boiler;
                }

                if (boiler.Consumption < lowestConsumptionBoiler.Consumption)
                {
                    lowestConsumptionBoiler = boiler;
                }
            }
            return Ok(lowestConsumptionBoiler);
        }
        else
        {
            Console.WriteLine($"Exception in Controller/lowestConsumptionBoiler");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("allActiveGasBoilers")]
    public async Task<IActionResult> GetAllActiveGasBoilers()
    {
        var actionResult = await GetAllGasBoilers();
        if (actionResult is OkObjectResult)
        {
            OkObjectResult OkResult = (OkObjectResult)actionResult;
            List<GasBoilerDTO> boilers = (List<GasBoilerDTO>)OkResult.Value;
            List<GasBoilerDTO> Activeboilers = new List<GasBoilerDTO>();

            foreach(GasBoilerDTO boiler in boilers)
            {
                if (boiler.Active)
                {
                    Activeboilers.Add(boiler);
                }
            }
            return Ok(Activeboilers);
        }
        else
        {
            Console.WriteLine($"Exception in Controller/allActiveGasBoilers");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("allActiveOilBoilers")]
    public async Task<IActionResult> GetAllActiveOilBoilers()
    {
        var actionResult = await GetAllOilBoilers();
        if (actionResult is OkObjectResult)
        {
            OkObjectResult OkResult = (OkObjectResult)actionResult;
            List<OilBoilerDTO> boilers = (List<OilBoilerDTO>)OkResult.Value;
            List<OilBoilerDTO> Activeboilers = new List<OilBoilerDTO>();

            foreach(OilBoilerDTO boiler in boilers)
            {
                if (boiler.Active)
                {
                    Activeboilers.Add(boiler);
                }
            }
            return Ok(Activeboilers);
        }
        else
        {
            Console.WriteLine($"Exception in Controller/allActiveOilBoilers");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("allActiveElectricBoilers")]
    public async Task<IActionResult> GetAllActiveElectricBoilers()
    {
        var actionResult = await GetAllElectricBoilers();
        if (actionResult is OkObjectResult)
        {
            OkObjectResult OkResult = (OkObjectResult)actionResult;
            List<ElectricBoilerDTO> boilers = (List<ElectricBoilerDTO>)OkResult.Value;
            List<ElectricBoilerDTO> Activeboilers = new List<ElectricBoilerDTO>();

            foreach(ElectricBoilerDTO boiler in boilers)
            {
                if (boiler.Active)
                {
                    Activeboilers.Add(boiler);
                }
            }
            return Ok(Activeboilers);
        }
        else
        {
            Console.WriteLine($"Exception in Controller/allActiveElectricBoilers");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("allActiveGasMotors")]
    public async Task<IActionResult> GetAllActiveGasMotors()
    {
        var actionResult = await GetAllGasMotors();
        if (actionResult is OkObjectResult)
        {
            OkObjectResult OkResult = (OkObjectResult)actionResult;
            List<GasMotorDTO> boilers = (List<GasMotorDTO>)OkResult.Value;
            List<GasMotorDTO> Activeboilers = new List<GasMotorDTO>();

            foreach(GasMotorDTO boiler in boilers)
            {
                if (boiler.Active)
                {
                    Activeboilers.Add(boiler);
                }
            }
            return Ok(Activeboilers);
        }
        else
        {
            Console.WriteLine($"Exception in Controller/allActiveGasMotors");
            return StatusCode(500, "Internal Server Error");
        }
    }
}