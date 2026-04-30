using Microsoft.AspNetCore.Mvc;
using Rdm.Api.Application.Interfaces;
using Rdm.Api.Application.Model;
using Rdm.Api.Application.Services;

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
    public async Task<IActionResult> Test()
    {
        try
        {
            List<OptimisationRun> optimisationRuns = await _optimisationResultService.GetAllOptimisationResults();
            
            return Ok(optimisationRuns);
        }
        catch (Exception e)
        {
            _logger.LogError($"Error in controller. Error getting all optimisation results: {e.Message}, {e.GetType()}");
            return StatusCode(500);
        }
        
    }
}