using Microsoft.AspNetCore.Mvc;
using Opt.Api.Application.Services;
using Opt.Api.DTOs;

namespace Opt.Api.Controllers;

[Route("api/[controller]")]
[ApiController]

public class OptimizationResults : Controller
{
    private readonly Optimizer _optimizer;
    private readonly ILogger<OptimizationResults> _logger;
    
    public OptimizationResults(Optimizer optimizer, ILogger<OptimizationResults> logger)
    {
        _optimizer = optimizer;
        _logger = logger;
    }
    
    [HttpPost("optimize")]
    public async Task<IActionResult> Optimize(
        [FromBody] OptimizationRequestDto request,  
        [FromQuery] int periodId, //e.g. /api/OptimizationResults/optimize?periodId=2 when dealing with query of POST req
        [FromQuery] int optRunId, //e.g. /api/OptimizationResults/optimize?periodId=1&optRunId=1 when dealing with query of POST req
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _optimizer.OptimizeAsync(request, periodId, optRunId, cancellationToken);
            return Ok(result);
        }
        catch (ArgumentException e)
        {
            _logger.LogWarning(e, "Invalid optimization request in Controller/optimize");
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception in Controller/optimize");
            return StatusCode(500, "Internal Server Error");
        }
    }
}
