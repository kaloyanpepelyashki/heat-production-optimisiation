using Microsoft.AspNetCore.Mvc;
using Rdm.Api.Application.Interfaces;
using Rdm.Api.Application.Model;
using Rdm.Api.Application.Services.Helpers;
using Rdm.Api.Inrastructure.DTOs;

namespace Rdm.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CreateController : Controller
{
    
    private readonly ILogger<CreateController> _logger;
    private readonly IOptimisationResultService _optimisationResultService;
    private readonly IOptimiserService _optimiserService;
    
    public CreateController(IOptimisationResultService optimisationResultService, IOptimiserService optimiserService, ILogger<CreateController> logger)
    {
        _optimisationResultService = optimisationResultService;
        _optimiserService = optimiserService;
        _logger = logger;
    }
    
    [HttpPost("/optimisation")]
    public async Task<IActionResult> RequestOptimisation([FromBody] OptimisationRequestDto optimisationRequestDto)
    {
        try
        {
            //TODO Implement proper checks if the response is actually populated
            var response = await _optimiserService.RequestOptimisation(optimisationRequestDto);
            
            OptimisationRun optimisationDomainModel = OptimisationModelsMapper.ToDomain(response);
            bool optimisationSaveResult = await _optimisationResultService.SaveOptimisationRun(optimisationDomainModel);

            if (!optimisationSaveResult)
            {
                _logger.LogError("Error saving optimsisation result. Optimisation result not saved to database");
            }
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500);
        }
    }
}