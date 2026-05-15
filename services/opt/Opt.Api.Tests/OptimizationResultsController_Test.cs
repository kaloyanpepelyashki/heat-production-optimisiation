using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opt.Api.Application.Interfaces;
using Opt.Api.Application.Services;
using Opt.Api.Controllers;
using Opt.Api.DTOs;

namespace Opt.Api.Tests;

public class OptimizationResultsController_Test
{
    private readonly Mock<IAssetDataProvider> _mockAssetProvider;
    private readonly Mock<ISourceDataProvider> _mockSourceProvider;
    private readonly Optimizer _optimizer;
    private readonly OptimizationResults _controller;

    public OptimizationResultsController_Test()
    {
        _mockAssetProvider = new Mock<IAssetDataProvider>();
        _mockSourceProvider = new Mock<ISourceDataProvider>();
        _optimizer = new Optimizer(_mockAssetProvider.Object, _mockSourceProvider.Object, NullLogger<Optimizer>.Instance);
        _controller = new OptimizationResults(_optimizer, NullLogger<OptimizationResults>.Instance);
    }

    [Fact]
    public async Task Optimize_ReturnsOk_WhenSuccessful()
    {
        var request = new OptimizationRequestDto 
        { 
            ScenarioId = 1, 
            PeriodId = 1, 
            TimeFrom = new DateTime(2025, 1, 1), 
            TimeTo = new DateTime(2025, 1, 2) 
        };
        
        _mockAssetProvider.Setup(o => o.GetAssetDataAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new Opt.Api.Domain.Models.AssetDataBundle());

        _mockSourceProvider.Setup(x => x.GetSourceDataAsync(It.IsAny<CancellationToken>()))
                           .ReturnsAsync(new List<Opt.Api.Domain.Models.SourceDataPoint>());

        var result = await _controller.Optimize(request, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task Optimize_Returns502_WhenExternalDataFetchFails()
    {
        var request = new OptimizationRequestDto();

        _mockAssetProvider.Setup(o => o.GetAssetDataAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                          .ThrowsAsync(new Exception("Provider unreachable"));

        var result = await _controller.Optimize(request, CancellationToken.None);

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(502, statusCodeResult.StatusCode);
    }
}