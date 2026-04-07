using Moq;
using Opt.Api;
using Opt.Api.Application.Interfaces;
using Opt.Api.Application.Services;
using Opt.Api.Infrastructure.Persistence.PersistenceModels;

namespace Opt.Api.Tests;

public class NetProductionCostService_Test
{
    private readonly Mock<INetProductionCostRepository> repoNetProductionCost = new Mock<INetProductionCostRepository>();

    private List<NetProductionCostPersistence> sampleData = new List<NetProductionCostPersistence>
    {
        new NetProductionCostPersistence
        {
            Id = 1,
            PeriodId = 1,
            TimeFrom = new DateTime(2026, 4, 4),
            TimeTo = new DateTime(2026, 4, 5),
            NetProdcutionCost = 2000.0,
        },
    };

    [Fact]
    public async Task GetAllNetProductionCostAsync_Return_All_Data()
    {
        //Arrange
        repoNetProductionCost.Setup(x => x.GetAllNetProductionCostAsync()).ReturnsAsync(sampleData);

        var service = new NetProductionCostService(repoNetProductionCost.Object);

        //Act
        var result = await service.GetAllNetProductionCostAsync();

        //Assert
        Assert.NotEmpty(result);
        Assert.Equal(1, result[0].Id);
        Assert.Equal(1, result[0].PeriodId);
        Assert.Equal(2000.0, result[0].NetProdcutionCost);
    }
}