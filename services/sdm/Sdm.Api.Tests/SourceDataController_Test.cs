namespace Sdm.Api.Tests;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Sdm.Api.Application.Interfaces;
using Sdm.Api.Controllers;
using Sdm.Api.Infrastructure.Persistence.PersistenceModels;

public class SourceDataController_Test
{
    private readonly Mock<ISourceDataService> _mockService;
    private readonly GetAllSourceDataController _controller;

    public SourceDataController_Test()
    {
        this._mockService = new Mock<ISourceDataService>();
        this._controller = new GetAllSourceDataController(this._mockService.Object);
    }

    [Fact]
    public async Task GetAllSourceData_ReturnsOk_WithData()
    {
        var mockData = new List<SourceDataPersistence>
        {
            new SourceDataPersistence { Id = 1, PeriodId = 1, HeatDemand = 10, ElectricityPrice = 100 },
        };
        this._mockService.Setup(s => s.GetAllSourceData()).ReturnsAsync(mockData);

        var result = await this._controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetAllSourceData_ReturnsNotFound_WhenNoDataAndExceptionThrown()
    {
        this._mockService.Setup(s => s.GetAllSourceData())
                    .ThrowsAsync(new Sdm.Api.Application.Exceptions.NoDataFoundException("No data"));

        var result = await this._controller.GetAll();

        Assert.IsType<NotFoundObjectResult>(result);
    }
}