using Microsoft.AspNetCore.Mvc;
using Opt.Api.Application.Exceptions;
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
          
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _optimizer.OptimizeAsync(request, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException e)
        {
            _logger.LogWarning(e, "Insufficient capacity in Controller/optimize");
            return UnprocessableEntity(e.Message);
        }
        catch (ExternalDataFetchException e)
        {
            _logger.LogError(e, "Failed to fetch external data in Controller/optimize");
            return StatusCode(502, "Bad Gateway: could not retrieve required data from upstream service.");
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
