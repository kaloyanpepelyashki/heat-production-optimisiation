using Microsoft.AspNetCore.Mvc;
using Sdm.Api.Application.Interfaces;
using Sdm.Api.Infrastructure.DTOs;
using Sdm.Api.Infrastructure.Persistence.PersistenceModels;

namespace Sdm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GetAllSourceDataController : Controller
{
    private ISourceDataService _sourceDataService;
    
    public GetAllSourceDataController(ISourceDataService sourceDataService)
    {
        _sourceDataService = sourceDataService;
    }
    
    [HttpGet("/getAll")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            List<SourceDataPersistence> sourceDataResult = await _sourceDataService.GetAllSourceData(); 
            
            List<SourceDataDTO> sourceDataDTOs = sourceDataResult.Select(res =>
            
                new SourceDataDTO
                {
                    Id = res.Id,
                    PeriodId = res.PeriodId,
                    TimeFrom = res.TimeFrom,
                    TimeTo = res.TimeTo,
                    HeatDemand = res.HeatDemand,
                    ElectricityPrice = res.ElectricityPrice,
                }).ToList();
            
            return Ok(sourceDataDTOs);
        }
        catch (Exception e)
        {
            return StatusCode(500, "Error Getting All SourceData");
        }
    }
}