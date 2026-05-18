using Microsoft.AspNetCore.Mvc;
using Rdm.Api.Application.Exceptions;
using Rdm.Api.Application.Interfaces;
using Rdm.Api.Application.Model;
using Rdm.Api.Application.Services;
using Rdm.Api.Inrastructure.API;

namespace Rdm.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class GetResult : Controller
{   
    private readonly ILogger<GetResult> _logger;
    private readonly IOptimisationResultService _optimisationResultService;

    public GetResult(IOptimisationResultService optimisationResultService, ILogger<GetResult> logger)
    {
        _optimisationResultService = optimisationResultService;
        _logger = logger;
    }
    
    [HttpGet("/allOptimisationRuns")]
    public async Task<IActionResult> GetAllOptimisationRuns()
    {
        try
        {
            List<OptimisationRun> optimisationRuns = await _optimisationResultService.GetAllOptimisationResults();

            if (optimisationRuns.Count != 0)
            {
                ApiResponseModel<List<OptimisationRun>> returnObject = new ApiResponseModel<List<OptimisationRun>>("Success", optimisationRuns, optimisationRuns.Count);
                return Ok(returnObject);
            }
            else
            {
                ApiResponseModel<List<OptimisationRun>> returnObject = new ApiResponseModel<List<OptimisationRun>>("No data found", [], 0);
                return Ok(returnObject);
                
            }
            
            
        }
        catch (DatabaseOperationException e)
        {
            _logger.LogError(e, "Database operation error getting all optimisation results");
            ApiResponseModel<List<OptimisationRun>> returnObject = new ApiResponseModel<List<OptimisationRun>>("Internal Server error", [], 0, "Database error retrieving optimisation results");
            return StatusCode(500, returnObject);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting all optimisation results");
            ApiResponseModel<List<OptimisationRun>> returnObject = new ApiResponseModel<List<OptimisationRun>>("Internal Server error", [], 0, "Error in controller. Error getting all optimisation results");
            return StatusCode(500, returnObject);
        }
        
    }
}