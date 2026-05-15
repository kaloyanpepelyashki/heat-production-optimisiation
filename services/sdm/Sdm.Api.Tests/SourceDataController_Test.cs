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
    private readonly SourceDataController _controller;

    public SourceDataController_Test()
    {
        _mockService = new Mock<ISourceDataService>();
        _controller = new SourceDataController(_mockService.Object, NullLogger<SourceDataController>.Instance);
    }

    [Fact]
    public async Task GetAllSourceData_ReturnsOk_WithData()
    {
        var mockData = new List<SourceDataPersistence>
        {
            new SourceDataPersistence { Id = 1, PeriodId = 1, HeatDemand = 10, ElectricityPrice = 100 }
        };
        _mockService.Setup(s => s.GetAllSourceData()).ReturnsAsync(mockData);

        var result = await _controller.GetAllSourceData();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedData = Assert.IsAssignableFrom<IEnumerable<SourceDataPersistence>>(okResult.Value);
        Assert.Single(returnedData);
    }

    [Fact]
    public async Task GetAllSourceData_ReturnsNotFound_WhenNoDataAndExceptionThrown()
    {
        _mockService.Setup(s => s.GetAllSourceData())
                    .ThrowsAsync(new Sdm.Api.Application.Exceptions.NoDataFoundException("No data"));

        await Assert.ThrowsAsync<Sdm.Api.Application.Exceptions.NoDataFoundException>(() => _controller.GetAllSourceData());
    }
}