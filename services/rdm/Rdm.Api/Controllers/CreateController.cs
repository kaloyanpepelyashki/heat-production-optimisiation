using Microsoft.AspNetCore.Mvc;
using Rdm.Api.Application.Interfaces;
using Rdm.Api.Application.Model;
using Rdm.Api.Application.Services.Helpers;
using Rdm.Api.Inrastructure.API;
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
            var response = await _optimiserService.RequestOptimisation(optimisationRequestDto);

            if (response == null || response.OptimisationResultsHourly.Count == 0)
            {
                ApiResponseModel<OptimisationRun> returnObject = new ApiResponseModel<OptimisationRun>("No data found", null, 0);
                return StatusCode(409);
            }
            
            OptimisationRun optimisationDomainModel = OptimisationModelsMapper.ToDomain(response);
            bool optimisationSaveResult = await _optimisationResultService.SaveOptimisationRun(optimisationDomainModel);

            if (!optimisationSaveResult)
            {
                _logger.LogError("Error saving optimsisation result. Optimisation result not saved to database");
                ApiResponseModel<OptimisationRun> returnObject = new ApiResponseModel<OptimisationRun>("Internal Server Error", null, 0, "Failed to save optimisation result to database.");
                return StatusCode(500, returnObject);
            }
            else
            {
                ApiResponseModel<OptimisationRun> returnObject = new ApiResponseModel<OptimisationRun>("Success", optimisationDomainModel, 0, "OK");
                return Ok(returnObject);
            }
            
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Insufficient boiler capacity");
            return UnprocessableEntity(new ApiResponseModel<OptimisationRun>(ex.Message, null, 0, ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            ApiResponseModel<OptimisationRun> returnObject = new ApiResponseModel<OptimisationRun>("Internal Server Error", null, 0, "Error requesting optimisation.");
            return StatusCode(500, returnObject);
        }
    }
}