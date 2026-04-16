using Microsoft.AspNetCore.Mvc;
using Opt.Api.Infrastructure.DTOs;
using Opt.Api.Application.Interfaces;
using Opt.Api.Infrastructure.Persistence.PersistenceModels;

namespace Opt.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GetNetProductionCostController : Controller
{
    private readonly INetProductionCostService _netProductionCostService;

    public GetNetProductionCostController(INetProductionCostService netProductionCostService)
    {
        _netProductionCostService = netProductionCostService;
    }

    [HttpGet("getAllNPC")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            List<NetProductionCostPersistence> netProductionCostResult = await _netProductionCostService.GetAllNetProductionCostAsync();

            List<NetProductionCostDTO> netProductionCostDTOs = netProductionCostResult.Select(res =>

                new NetProductionCostDTO
                {
                    Id = res.Id,
                    PeriodId = res.PeriodId,
                    TimeFrom = res.TimeFrom,
                    TimeTo = res.TimeTo,
                    NetProductionCost = res.NetProdcutionCost,
                }).ToList();

            return Ok(netProductionCostDTOs);
        }
        catch (Exception e)
        {
            return StatusCode(500, "Error Getting All SourceData");
        }
    }
}
