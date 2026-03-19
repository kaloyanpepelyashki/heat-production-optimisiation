using Am.Api.Application.Interfaces;
using Am.Api.Model.DTOs;
using Microsoft.AspNetCore.Mvc;

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
            List<GasBoilerPersistence> gasBoilersResult = await _productionUnitService.GetAllGasBoilersAsync();
            
            return Ok(gasBoilersResult);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}