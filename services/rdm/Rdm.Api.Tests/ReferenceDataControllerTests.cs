namespace Rdm.Api.Tests;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Rdm.Api.Application.Exceptions;
using Rdm.Api.Application.Interfaces;
using Rdm.Api.Application.Model;
using Rdm.Api.Controllers;
using Rdm.Api.Inrastructure.API;
using Xunit;

public class GetResultControllerTests
{
    private readonly Mock<IOptimisationResultService> _mockService;
    private readonly GetResult _controller;

    public GetResultControllerTests()
    {
        this._mockService = new Mock<IOptimisationResultService>();
        this._controller = new GetResult(this._mockService.Object, NullLogger<GetResult>.Instance);
    }

    [Fact]
    public async Task GetAllOptimisationRuns_ReturnsOk_WithData_Positive()
    {
        var runs = new List<OptimisationRun>
        {
            new OptimisationRun { Id = 1 },
        };
        this._mockService.Setup(s => s.GetAllOptimisationResults()).ReturnsAsync(runs);

        var result = await this._controller.GetAllOptimisationRuns();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResponse = Assert.IsType<ApiResponseModel<List<OptimisationRun>>>(okResult.Value);
        Assert.Equal("Success", apiResponse.Message);
        Assert.Single(apiResponse.Data);
    }

    [Fact]
    public async Task GetAllOptimisationRuns_ReturnsOk_WithNoData_Edge()
    {
        this._mockService.Setup(s => s.GetAllOptimisationResults()).ReturnsAsync(new List<OptimisationRun>());

        var result = await this._controller.GetAllOptimisationRuns();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResponse = Assert.IsType<ApiResponseModel<List<OptimisationRun>>>(okResult.Value);
        Assert.Equal("No data found", apiResponse.Message);
        Assert.Empty(apiResponse.Data);
    }

    [Fact]
    public async Task GetAllOptimisationRuns_Returns500_OnException_Negative()
    {
        this._mockService.Setup(s => s.GetAllOptimisationResults()).ThrowsAsync(new Exception("DB Failure"));

        var result = await this._controller.GetAllOptimisationRuns();

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        var apiResponse = Assert.IsType<ApiResponseModel<List<OptimisationRun>>>(statusCodeResult.Value);
        Assert.Equal("Internal Server error", apiResponse.Message);
    }

    [Fact]
    public async Task GetAllOptimisationRuns_Returns500_OnDatabaseOperationException_Negative()
    {
        this._mockService.Setup(s => s.GetAllOptimisationResults()).ThrowsAsync(new DatabaseOperationException("DB operation failed"));

        var result = await this._controller.GetAllOptimisationRuns();

        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        var apiResponse = Assert.IsType<ApiResponseModel<List<OptimisationRun>>>(statusCodeResult.Value);
        Assert.Equal("Internal Server error", apiResponse.Message);
    }
}