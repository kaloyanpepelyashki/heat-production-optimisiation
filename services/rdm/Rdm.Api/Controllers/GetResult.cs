namespace Rdm.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Rdm.Api.Application.Exceptions;
using Rdm.Api.Application.Interfaces;
using Rdm.Api.Application.Model;
using Rdm.Api.Application.Services;
using Rdm.Api.Inrastructure.API;

[Route("api/[controller]")]
[ApiController]
public class GetResult : Controller
{
    private readonly ILogger<GetResult> _logger;
    private readonly IOptimisationResultService _optimisationResultService;

    public GetResult(IOptimisationResultService optimisationResultService, ILogger<GetResult> logger)
    {
        this._optimisationResultService = optimisationResultService;
        this._logger = logger;
    }

    [HttpGet("/allOptimisationRuns")]
    public async Task<IActionResult> GetAllOptimisationRuns()
    {
        try
        {
            List<OptimisationRun> optimisationRuns = await this._optimisationResultService.GetAllOptimisationResults();

            if (optimisationRuns.Count != 0)
            {
                ApiResponseModel<List<OptimisationRun>> returnObject = new ApiResponseModel<List<OptimisationRun>>("Success", optimisationRuns, optimisationRuns.Count);
                return this.Ok(returnObject);
            }
            else
            {
                ApiResponseModel<List<OptimisationRun>> returnObject = new ApiResponseModel<List<OptimisationRun>>("No data found", [], 0);
                return this.Ok(returnObject);
            }
        }
        catch (DatabaseOperationException e)
        {
            this._logger.LogError(e, "Database operation error getting all optimisation results");
            ApiResponseModel<List<OptimisationRun>> returnObject = new ApiResponseModel<List<OptimisationRun>>("Internal Server error", [], 0, "Database error retrieving optimisation results");
            return this.StatusCode(500, returnObject);
        }
        catch (Exception e)
        {
            this._logger.LogError(e, "Error getting all optimisation results");
            ApiResponseModel<List<OptimisationRun>> returnObject = new ApiResponseModel<List<OptimisationRun>>("Internal Server error", [], 0, "Error in controller. Error getting all optimisation results");
            return this.StatusCode(500, returnObject);
        }
    }
}