namespace Sdm.Api.Tests;

using Moq;
using Sdm.Api.Application.Exceptions;
using Sdm.Api.Application.Interfaces;
using Sdm.Api.Application.Services;
using Sdm.Api.Infrastructure.Persistence.PersistenceModels;

public class SourceDataService_Test
{
    Mock<ISourceDataRepository> repoSourceData = new Mock<ISourceDataRepository>();

    [Fact]
    public async Task GetAllSourceData_Returns_List_Of_SourceData()
    {
        this.repoSourceData.Setup(x => x.GetAllSourceData()).ReturnsAsync(new List<SourceDataPersistence>
        {
            new SourceDataPersistence
            {
                Id = 1,
                PeriodId = 1,
                TimeFrom = new DateTime(2024, 1, 1, 0, 0, 0),
                TimeTo = new DateTime(2024, 1, 1, 1, 0, 0),
                HeatDemand = 15.5,
                ElectricityPrice = 100.0,
            },
            new SourceDataPersistence
            {
                Id = 2,
                PeriodId = 1,
                TimeFrom = new DateTime(2024, 1, 1, 1, 0, 0),
                TimeTo = new DateTime(2024, 1, 1, 2, 0, 0),
                HeatDemand = 16.2,
                ElectricityPrice = 95.0,
            },
        });

        var service = new SourceDataService(this.repoSourceData.Object);

        var result = await service.GetAllSourceData();

        Assert.NotEmpty(result);
        Assert.IsType<List<SourceDataPersistence>>(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Id);
        Assert.Equal(1, result[0].PeriodId);
        Assert.Equal(15.5, result[0].HeatDemand);
        Assert.Equal(100.0, result[0].ElectricityPrice);
    }

    [Fact]
    public async Task GetAllSourceData_Returns_SingleItem_List()
    {
        this.repoSourceData.Setup(x => x.GetAllSourceData()).ReturnsAsync(new List<SourceDataPersistence>
        {
            new SourceDataPersistence
            {
                Id = 1,
                PeriodId = 2,
                TimeFrom = new DateTime(2024, 6, 15, 8, 0, 0),
                TimeTo = new DateTime(2024, 6, 15, 9, 0, 0),
                HeatDemand = 20.0,
                ElectricityPrice = 110.5,
            },
        });

        var service = new SourceDataService(this.repoSourceData.Object);

        var result = await service.GetAllSourceData();

        Assert.Single(result);
        Assert.Equal(1, result[0].Id);
        Assert.Equal(2, result[0].PeriodId);
        Assert.Equal(20.0, result[0].HeatDemand);
        Assert.Equal(110.5, result[0].ElectricityPrice);
    }

    [Fact]
    public async Task GetAllSourceData_ThrowsNoDataFoundException_WhenRepositoryThrows()
    {
        this.repoSourceData.Setup(x => x.GetAllSourceData())
            .ThrowsAsync(new NoDataFoundException("No data found when Getting all source data"));

        var service = new SourceDataService(this.repoSourceData.Object);

        await Assert.ThrowsAsync<NoDataFoundException>(() => service.GetAllSourceData());
    }

    [Fact]
    public async Task GetAllSourceData_ThrowsException_WhenRepositoryThrowsGenericException()
    {
        this.repoSourceData.Setup(x => x.GetAllSourceData())
            .ThrowsAsync(new Exception("Unexpected database error"));

        var service = new SourceDataService(this.repoSourceData.Object);

        await Assert.ThrowsAsync<Exception>(() => service.GetAllSourceData());
    }

    [Fact]
    public async Task GetAllSourceData_CallsRepository_ExactlyOnce()
    {
        this.repoSourceData.Setup(x => x.GetAllSourceData()).ReturnsAsync(new List<SourceDataPersistence>
        {
            new SourceDataPersistence { Id = 1, PeriodId = 1 },
        });

        var service = new SourceDataService(this.repoSourceData.Object);

        await service.GetAllSourceData();

        this.repoSourceData.Verify(x => x.GetAllSourceData(), Times.Once);
    }
}
