namespace Rdm.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Rdm.Api.Application.Interfaces;
using Rdm.Api.Domain.Models;
using Rdm.Api.Infrastructure.DTOs;

[Route("api/[controller]")]
[ApiController]
public class OptimizationResults : Controller
{
    private readonly IOptimizationResultService _service;

    public OptimizationResults(IOptimizationResultService service)
    {
        _service = service;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllResults()
    {
        var results = await _service.GetAllResultsAsync();
        var resultDtos = results.Select(r => MapToDto(r)).ToList();
        return Ok(resultDtos);
    }

    [HttpGet("period/{period}")]
    public async Task<IActionResult> GetResultsByPeriod(string period)
    {
        var results = await _service.GetResultsByPeriodAsync(period);
        var resultDtos = results.Select(r => MapToDto(r)).ToList();
        return Ok(resultDtos);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateResult([FromBody] CreateOptimizationResultDTO createDto)
    {
        var result = new OptimizationResult
        {
            ProductionUnit = createDto.ProductionUnit,
            TotalHeat = createDto.TotalHeat,
            TotalCost = createDto.TotalCost,
            TotalEmissions = createDto.TotalEmissions
        };

        var createdResult = await _service.CreateResultAsync(result);
        return Created(string.Empty, MapToDto(createdResult));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateResult(int id, [FromBody] CreateOptimizationResultDTO updateDto)
    {
        var result = new OptimizationResult
        {
            ProductionUnit = updateDto.ProductionUnit,
            TotalHeat = updateDto.TotalHeat,
            TotalCost = updateDto.TotalCost,
            TotalEmissions = updateDto.TotalEmissions
        };

        var updatedResult = await _service.UpdateResultAsync(id, result);

        return Ok(MapToDto(updatedResult));
    }

    [HttpGet("/test")]
    public IActionResult Test()
    {
        return Ok(new { message = "RDM API is running successfully!" });
    }

    private OptimizationResultDTO MapToDto(OptimizationResult result)
    {
        return new OptimizationResultDTO
        {
            Id = result.Id,
            ProductionUnit = result.ProductionUnit,
            TotalHeat = result.TotalHeat,
            TotalCost = result.TotalCost,
            TotalEmissions = result.TotalEmissions,
            CreatedAt = result.CreatedAt
        };
    }
}