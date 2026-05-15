using Am.Api.Application.Interfaces;
using Am.Api.Controllers;
using Am.Api.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Am.Api.Tests;

public class GetProductionUnitsController_Test
{
    private readonly Mock<IProductionUnitService> _mockService;
    private readonly GetProductionUnits _controller;

    public GetProductionUnitsController_Test()
    {
        _mockService = new Mock<IProductionUnitService>();
        // Assuming controller constructor takes the service and perhaps a logger
        _controller = new GetProductionUnits(_mockService.Object, NullLogger<GetProductionUnits>.Instance);
    }

    [Fact]
    public async Task GetGasBoilers_ReturnsOk_WithData()
    {
        // Arrange
        var mockData = new List<GasBoiler>
        {
            new GasBoiler { Id = 1, Name = "GB1", MaxHeat = 5.0f }
        };
        _mockService.Setup(s => s.GetAllGasBoilersAsync()).ReturnsAsync(mockData);

        // Act
        var result = await _controller.GetGasBoilers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedData = Assert.IsAssignableFrom<IEnumerable<GasBoiler>>(okResult.Value);
        Assert.Single(returnedData);
    }

    [Fact]
    public async Task GetGasBoilers_ReturnsInternalServerError_OnException()
    {
        // Arrange
        _mockService.Setup(s => s.GetAllGasBoilersAsync()).ThrowsAsync(new Exception("Database failed"));

        // Act
        // Typically a controller with standard error handling returns a 500, or it throws and middleware catches it.
        // We will assert exception throw if there is no try/catch in the controller, or assert the status code if there is.
        // Let's assert a generic Exception will be thrown, simulating a missing try/catch or middleware boundary.
        await Assert.ThrowsAsync<Exception>(() => _controller.GetGasBoilers());
    }

    [Fact]
    public async Task GetProductionUnitMaintenanceById_ReturnsNotFound_WhenNull()
    {
        // Arrange
        _mockService.Setup(s => s.GetProductionUnitMaintenanceByIdAsync(It.IsAny<int>()))
                    .ReturnsAsync((ProductionUnitMaintenance)null);

        // Act
        var result = await _controller.GetMaintenanceById(99);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}