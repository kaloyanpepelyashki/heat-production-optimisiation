using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Sdm.Api.Application.Interfaces;
using Sdm.Api.Controllers;
using Sdm.Api.Infrastructure.Persistence.PersistenceModels;

namespace Sdm.Api.Tests;

public class SourceDataController_Test
{
    private readonly Mock<ISourceDataService> _mockService;
    private readonly GetAllSourceDataController _controller;

    public SourceDataController_Test()
    {
        _mockService = new Mock<ISourceDataService>();
        _controller = new GetAllSourceDataController(_mockService.Object);
    }

    [Fact]
    public async Task GetAllSourceData_ReturnsOk_WithData()
    {
        var mockData = new List<SourceDataPersistence>
        {
            new SourceDataPersistence { Id = 1, PeriodId = 1, HeatDemand = 10, ElectricityPrice = 100 }
        };
        _mockService.Setup(s => s.GetAllSourceData()).ReturnsAsync(mockData);

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetAllSourceData_ReturnsNotFound_WhenNoDataAndExceptionThrown()
    {
        _mockService.Setup(s => s.GetAllSourceData())
                    .ThrowsAsync(new Sdm.Api.Application.Exceptions.NoDataFoundException("No data"));

        var result = await _controller.GetAll();

        Assert.IsType<NotFoundObjectResult>(result);
    }
}