using Moq;
using Opt.Api;
using Opt.Api.Application.Interfaces;
using Opt.Api.Application.Services;
using Opt.Api.Infrastructure.Persistence.PersistenceModels;

namespace Opt.Api.Tests;

public class SelectedProductionUnitsService_Test
{
    private readonly Mock<ISelectedProductionUnitsRepository> repoSelectedProductionUnits = new Mock<ISelectedProductionUnitsRepository>();

    private List<SelectedProductionUnitsPersistence> sampleData = new List<SelectedProductionUnitsPersistence>
    {
        new SelectedProductionUnitsPersistence
        {
            Id = 1,
            PeriodId = 1,
            TimeFrom = new DateTime(2026, 4, 4),
            TimeTo = new DateTime(2026, 4, 5),
            SelectedProductionUnitsNames = new List<string> {"GB1", "GB2", "GB3", "OB1"},
        },
    };

    [Fact]
    public async Task GetAllSelectedProductionUnitsAsync_Return_All_Data()
    {
        //Arrange
        repoSelectedProductionUnits.Setup(x => x.GetAllSelectedProductionUnitsAsync()).ReturnsAsync(sampleData);

        var service = new SelectedProductionUnitsService(repoSelectedProductionUnits.Object);

        //Act
        var result = await service.GetAllSelectedProductionUnitsAsync();

        //Assert
        Assert.NotEmpty(result);
        Assert.Equal(1, result[0].Id);
        Assert.Equal(1, result[0].PeriodId);
        Assert.Equal(["GB1", "GB2", "GB3", "OB1"], result[0].SelectedProductionUnitsNames);
    }
}