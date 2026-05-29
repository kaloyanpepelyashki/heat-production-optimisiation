namespace Am.Api.Tests;

using Am.Api.Application.Exceptions;
using Am.Api.Application.Interfaces;
using Am.Api.Controllers;
using Am.Api.Domain.Models;
using Am.Api.Infrastructure.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

public class GetProductionUnitsController_Test
{
    private readonly Mock<IProductionUnitService> _mockService;
    private readonly GetProductionUnits _controller;

    public GetProductionUnitsController_Test()
    {
        this._mockService = new Mock<IProductionUnitService>();
        this._controller = new GetProductionUnits(this._mockService.Object, NullLogger<GetProductionUnits>.Instance);
    }

    [Fact]
    public async Task GetGasBoilers_ReturnsOk_WithData()
    {
        var mockData = new List<GasBoiler>
        {
            new GasBoiler { Id = 1, Name = "GB1", MaxHeat = 5.0f },
        };
        this._mockService.Setup(s => s.GetAllGasBoilersAsync()).ReturnsAsync(mockData);

        var result = await this._controller.GetAllGasBoilers();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedData = Assert.IsAssignableFrom<IEnumerable<GasBoilerDTO>>(okResult.Value);
        Assert.Single(returnedData);
    }

    [Fact]
    public async Task GetGasBoilers_ReturnsInternalServerError_OnException()
    {
        this._mockService.Setup(s => s.GetAllGasBoilersAsync()).ThrowsAsync(new Exception("Database failed"));

        var result = await this._controller.GetAllGasBoilers();

        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    [Fact]
    public async Task GetProductionUnitMaintenanceById_ReturnsNotFound_WhenNotFound()
    {
        this._mockService.Setup(s => s.GetProductionUnitMaintenanceByIdAsync(It.IsAny<int>()))
                    .ThrowsAsync(new KeyNotFoundException("Not found"));

        var result = await this._controller.GetProductionUnitMaintenanceById(99);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetGasBoilers_ReturnsNotFound_WhenNoAssetsFound()
    {
        this._mockService.Setup(s => s.GetAllGasBoilersAsync())
                    .ThrowsAsync(new NoAssetsFoundException("No gas boilers found"));

        var result = await this._controller.GetAllGasBoilers();

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task PostProductionUnitMaintenance_ReturnsBadRequest_WhenArgumentNullException()
    {
        this._mockService.Setup(s => s.PostProductionUnitMaintenanceAsync(It.IsAny<ProductionUnitMaintenance>()))
                    .ThrowsAsync(new ArgumentNullException("maintenance"));

        var result = await this._controller.PostProductionUnitMaintenance(new ProductionUnitMaintenanceDTO());

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
