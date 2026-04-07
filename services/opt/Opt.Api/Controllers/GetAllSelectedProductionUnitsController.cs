using Microsoft.AspNetCore.Mvc;
using Opt.Api.Infrastructure.DTOs;
using Opt.Api.Application.Interfaces;
using Opt.Api.Infrastructure.Persistence.PersistenceModels;

namespace Opt.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GetSelectedProductionUnitsController : Controller
{
    private readonly ISelectedProductionUnitsService _selectedProductionUnitsService;

    public GetSelectedProductionUnitsController(ISelectedProductionUnitsService selectedProductionUnitsService)
    {
        _selectedProductionUnitsService = selectedProductionUnitsService;
    }

    [HttpGet("/getAll")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            List<SelectedProductionUnitsPersistence> selectedProductionUnitsResult = await _selectedProductionUnitsService.GetAllSelectedProductionUnitsAsync();

            List<SelectedProductionUnitsDTO> selectedProductionUnitsDTOs = selectedProductionUnitsResult.Select(res =>

                new SelectedProductionUnitsDTO
                {
                    Id = res.Id,
                    PeriodId = res.PeriodId,
                    TimeFrom = res.TimeFrom,
                    TimeTo = res.TimeTo,
                    SelectedProductionUnitsNames = res.SelectedProductionUnitsNames,
                }).ToList();
            return Ok(selectedProductionUnitsDTOs);
        }
        catch (Exception e)
        {
            return StatusCode(500, "Error Getting All SelectedProductionUnits");
        }
    }
}
