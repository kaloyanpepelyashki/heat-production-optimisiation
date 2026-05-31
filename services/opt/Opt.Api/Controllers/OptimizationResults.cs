namespace Opt.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Opt.Api.Application.Exceptions;
using Opt.Api.Application.Services;
using Opt.Api.Infrastructure.DTOs;

[Route("api/[controller]")]
[ApiController]

public class OptimizationResults : Controller
{
    private readonly Optimizer _optimizer;
    private readonly ILogger<OptimizationResults> _logger;

    public OptimizationResults(Optimizer optimizer, ILogger<OptimizationResults> logger)
    {
        this._optimizer = optimizer;
        this._logger = logger;
    }

    [HttpPost("optimize")]
    public async Task<IActionResult> Optimize(
        [FromBody] OptimizationRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await this._optimizer.OptimizeAsync(request, cancellationToken);
            return this.Ok(result);
        }
        catch (InvalidOperationException e)
        {
            this._logger.LogWarning(e, "Insufficient capacity in Controller/optimize");
            return this.UnprocessableEntity(e.Message);
        }
        catch (ExternalDataFetchException e)
        {
            this._logger.LogError(e, "Failed to fetch external data in Controller/optimize");
            return this.StatusCode(502, "Bad Gateway: could not retrieve required data from upstream service.");
        }
        catch (ArgumentException e)
        {
            this._logger.LogWarning(e, "Invalid optimization request in Controller/optimize");
            return this.BadRequest(e.Message);
        }
        catch (Exception e)
        {
            this._logger.LogError(e, "Exception in Controller/optimize");
            return this.StatusCode(500, "Internal Server Error");
        }
    }
}
