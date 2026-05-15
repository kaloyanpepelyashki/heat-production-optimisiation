using Am.Api.Application.Interfaces;
using Am.Api.Controllers;
using Am.Api.Domain.Models;
using Am.Api.Infrastructure.DTOs;
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
        _controller = new GetProductionUnits(_mockService.Object, NullLogger<GetProductionUnits>.Instance);
    }

    [Fact]
    public async Task GetGasBoilers_ReturnsOk_WithData()
    {
        var mockData = new List<GasBoiler>
        {
            new GasBoiler { Id = 1, Name = "GB1", MaxHeat = 5.0f }
        };
        _mockService.Setup(s => s.GetAllGasBoilersAsync()).ReturnsAsync(mockData);

        var result = await _controller.GetAllGasBoilers();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedData = Assert.IsAssignableFrom<IEnumerable<GasBoilerDTO>>(okResult.Value);
        Assert.Single(returnedData);
    }

    [Fact]
    public async Task GetGasBoilers_ReturnsInternalServerError_OnException()
    {
        _mockService.Setup(s => s.GetAllGasBoilersAsync()).ThrowsAsync(new Exception("Database failed"));

        var result = await _controller.GetAllGasBoilers();

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    [Fact]
    public async Task GetProductionUnitMaintenanceById_ReturnsNotFound_WhenNotFound()
    {
        _mockService.Setup(s => s.GetProductionUnitMaintenanceByIdAsync(It.IsAny<int>()))
                    .ThrowsAsync(new KeyNotFoundException("Not found"));

        var result = await _controller.GetProductionUnitMaintenanceById(99);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
