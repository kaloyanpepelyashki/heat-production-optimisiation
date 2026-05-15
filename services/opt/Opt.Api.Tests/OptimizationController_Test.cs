using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opt.Api.Application.Interfaces;
using Opt.Api.Controllers;
using Opt.Api.DTOs;

namespace Opt.Api.Tests;

public class OptimizationController_Test
{
    private readonly Mock<IOptimizer> _mockOptimizer;
    private readonly OptimizationController _controller;

    public OptimizationController_Test()
    {
        _mockOptimizer = new Mock<IOptimizer>();
        _controller = new OptimizationController(_mockOptimizer.Object, NullLogger<OptimizationController>.Instance);
    }

    [Fact]
    public async Task Optimize_ReturnsOk_WhenSuccessful()
    {
        var request = new OptimizationRequestDto 
        { 
            ScenarioId = 1, 
            PeriodId = 1, 
            TimeFrom = DateTime.Now, 
            TimeTo = DateTime.Now.AddDays(1) 
        };
        
        var expectedResult = new OptimizationResultDto();
        _mockOptimizer.Setup(o => o.OptimizeAsync(request, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(expectedResult);

        var result = await _controller.Optimize(request, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedResult, okResult.Value);
    }

    [Fact]
    public async Task Optimize_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        _controller.ModelState.AddModelError("ScenarioId", "Required");
        var request = new OptimizationRequestDto();

        var result = await _controller.Optimize(request, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}