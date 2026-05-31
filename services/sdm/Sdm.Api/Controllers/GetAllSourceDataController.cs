namespace Sdm.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Sdm.Api.Application.Exceptions;
using Sdm.Api.Application.Interfaces;
using Sdm.Api.Infrastructure.DTOs;
using Sdm.Api.Infrastructure.Persistence.PersistenceModels;

[ApiController]
[Route("api/[controller]")]
public class GetAllSourceDataController : Controller
{
    private ISourceDataService _sourceDataService;

    public GetAllSourceDataController(ISourceDataService sourceDataService)
    {
        this._sourceDataService = sourceDataService;
    }

    [HttpGet("/getAll")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            List<SourceDataPersistence> sourceDataResult = await this._sourceDataService.GetAllSourceData();

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
            return this.Ok(sourceDataDTOs);
        }
        catch (NoDataFoundException e)
        {
            return this.NotFound(e.Message);
        }
        catch (Exception)
        {
            return this.StatusCode(500, "Error Getting All SourceData");
        }
    }
}