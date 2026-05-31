namespace Am.Api.Controllers;

using Am.Api.Application.Exceptions;
using Am.Api.Application.Interfaces;
using Am.Api.Domain.Models;
using Am.Api.Infrastructure.DTOs;
using Am.Api.Model.DTOs;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class GetProductionUnits : Controller
{
    private readonly IProductionUnitService _productionUnitService;
    private readonly ILogger<GetProductionUnits> _logger;

    public GetProductionUnits(IProductionUnitService productionUnitService, ILogger<GetProductionUnits> logger)
    {
        this._productionUnitService = productionUnitService;
        this._logger = logger;
    }

    [HttpGet("allGasBoilers")]
    public async Task<IActionResult> GetAllGasBoilers()
    {
        try
        {
            List<GasBoiler> gasBoilersResult = await this._productionUnitService.GetAllGasBoilersAsync();

            List<GasBoilerDTO> gasBoilersDTOs = gasBoilersResult.Select(x => new GasBoilerDTO
            {
                Id = x.Id,
                Name = x.Name,
                MaxHeat = x.MaxHeat,
                ProductionCost = x.ProductionCost,
                Co2Emissions = x.Co2Emissions,
                GasConsumption = x.GasConsumption,
            }).ToList();

            return this.Ok(gasBoilersDTOs);
        }
        catch (NoAssetsFoundException e)
        {
            this._logger.LogWarning(e, "No gas boilers found");
            return this.NotFound(e.Message);
        }
        catch (Exception e)
        {
            this._logger.LogError(e, "Exception in Controller/allGasBoilers");
            return this.StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("allOilBoilers")]
    public async Task<IActionResult> GetAllOilBoilers()
    {
        try
        {
            List<OilBoiler> oilBoilerResult = await this._productionUnitService.GetAllOilBoilersAsync();

            List<OilBoilerDTO> oilBoilerDtos = oilBoilerResult.Select(obj => new OilBoilerDTO
            {
                Id = obj.Id,
                Name = obj.Name,
                MaxHeat = obj.MaxHeat,
                ProductionCost = obj.ProductionCost,
                Co2Emissions = obj.Co2Emissions,
                OilConsumption = obj.OilConsumption,
            }).ToList();

            return this.Ok(oilBoilerDtos);
        }
        catch (NoAssetsFoundException e)
        {
            this._logger.LogWarning(e, "No oil boilers found");
            return this.NotFound(e.Message);
        }
        catch (Exception e)
        {
            this._logger.LogError(e, "Exception in Controller/allOilBoilers");
            return this.StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("allElectricBoilers")]
    public async Task<IActionResult> GetAllElectricBoilers()
    {
        try
        {
            List<ElectricBoiler> electricBoilersResult =
                await this._productionUnitService.GetAllElectricBoilersAsync();

            List<ElectricBoilerDTO> electricBoilerDtos = electricBoilersResult.Select(elBoiler =>
                new ElectricBoilerDTO
                {
                    Id = elBoiler.Id,
                    Name = elBoiler.Name,
                    MaxHeat = elBoiler.MaxHeat,
                    ProductionCost = elBoiler.ProductionCost,
                    MaxElectricity = elBoiler.MaxElectricity,
                }).ToList();

            return this.Ok(electricBoilerDtos);
        }
        catch (NoAssetsFoundException e)
        {
            this._logger.LogWarning(e, "No electric boilers found");
            return this.NotFound(e.Message);
        }
        catch (Exception e)
        {
            this._logger.LogError(e, "Exception in Controller/allElectricBoilers");
            return this.StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("allGasMotors")]
    public async Task<IActionResult> GetAllGasMotors()
    {
        try
        {
            List<GasMotor> gasMotorResult = await this._productionUnitService.GetAllGasMotorsAsync();

            List<GasMotorDTO> gasMotorDtos = gasMotorResult.Select(gasMotor => new GasMotorDTO
            {
                Id = gasMotor.Id,
                Name = gasMotor.Name,
                MaxHeat = gasMotor.MaxHeat,
                ProductionCost = gasMotor.ProductionCost,
                MaxElectricity = gasMotor.MaxElectricity,
                Co2Emissions = gasMotor.Co2Emissions,
                GasConsumption = gasMotor.GasConsumption,
            }).ToList();

            return this.Ok(gasMotorDtos);
        }
        catch (NoAssetsFoundException e)
        {
            this._logger.LogWarning(e, "No gas motors found");
            return this.NotFound(e.Message);
        }
        catch (Exception e)
        {
            this._logger.LogError(e, "Exception in Controller/allGasMotors");
            return this.StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("productionUnitMaintenancesById/{id:int}")]
    public async Task<IActionResult> GetProductionUnitMaintenanceById(int id)
    {
        try
        {
            ProductionUnitMaintenance result =
                await this._productionUnitService.GetProductionUnitMaintenanceByIdAsync(id);

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

            return this.Ok(productionUnitMaintenanceDto);
        }
        catch (KeyNotFoundException e)
        {
            this._logger.LogWarning(e, "Production unit maintenance not found for id {Id}", id);
            return this.NotFound(e.Message);
        }
        catch (Exception e)
        {
            this._logger.LogError(e, "Exception in Controller/productionUnitMaintenancesById");
            return this.StatusCode(500, "Internal Server Error");
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
                await this._productionUnitService.PostProductionUnitMaintenanceAsync(maintenance);

            return this.Ok(result);
        }
        catch (ArgumentNullException e)
        {
            this._logger.LogWarning(e, "Null argument in Controller/productionUnitMaintenance");
            return this.BadRequest(e.Message);
        }
        catch (Exception e)
        {
            this._logger.LogError(e, "Exception in Controller/productionUnitMaintenance");
            return this.StatusCode(500, "Internal Server Error");
        }
    }
}