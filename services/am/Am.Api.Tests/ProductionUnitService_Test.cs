using Am.Api.Application.Interfaces;
using Am.Api.Application.Services;
using Am.Api.Model.DTOs;
using Am.Api.Domain.Models;
using Moq;

namespace Am.Api.Tests;

public class ProductionUnitService_Test
{
    Mock<IProductionUnitRepository<GasBoiler>> repoGasBoiler = new Mock<IProductionUnitRepository<GasBoiler>>();
    Mock<IProductionUnitRepository<GasMotor>> repoGasMotor = new Mock<IProductionUnitRepository<GasMotor>>();
    Mock<IProductionUnitRepository<ElectricBoiler>> repoElectricBoiler = new Mock<IProductionUnitRepository<ElectricBoiler>>();
    Mock<IProductionUnitRepository<OilBoiler>> repoOilBoiler = new Mock<IProductionUnitRepository<OilBoiler>>();
    Mock<IMaintenanceRepository> repoMaintenance = new Mock<IMaintenanceRepository>();
    
    [Fact]
    public async Task GetAllGasBoilersAsync_Returns_List_Of_Boilers()
    {
        
        //Arrange
        repoGasBoiler.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<GasBoiler>
        {   
            new GasBoiler{
            Id = 1,
            Name = "GB1",
            MaxHeat = 3.0f,
            ProductionCost = 510,
            Co2Emissions = 132,
            GasConsumption = 1.05f
            },
            new GasBoiler
            {
                Id = 2,
                Name = "GB1",
                MaxHeat = 4.1f,
                ProductionCost = 512,
                Co2Emissions = 141,
                GasConsumption = 1.09f 
            },
        });
        
        var service = new ProductionUnitService(repoGasBoiler.Object, repoOilBoiler.Object, repoElectricBoiler.Object, repoGasMotor.Object, repoMaintenance.Object);
        
        //Act
        var result = await service.GetAllGasBoilersAsync();
        
        
        Assert.NotEmpty(result);
        Assert.Equal(1, result[0].Id);
        Assert.Equal("GB1", result[0].Name);
        Assert.Equal(3.0f, result[0].MaxHeat);
    }

    [Fact]
    public async Task GetAllGasMotorsAsync_Returns_List_Of_Motors()
    {
        //Arrange
        repoGasMotor.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<GasMotor>
        {
            new GasMotor
            {
                Id = 1,
                Name = "GM1",
                MaxHeat = 5.3f,
                MaxElectricity = 3.9f,
                ProductionCost = 975,
                Co2Emissions = 227,
                GasConsumption = 1.05f,
            },
        });
        
        var service = new ProductionUnitService(repoGasBoiler.Object, repoOilBoiler.Object, repoElectricBoiler.Object, repoGasMotor.Object, repoMaintenance.Object);
        
        //Act
        var result = await service.GetAllGasMotorsAsync();
        
        //Assert
        Assert.IsType<List<GasMotor>>(result);
        Assert.Equal(1, result[0].Id);
        Assert.Equal("GM1", result[0].Name);
    }

    [Fact]
    public async Task GetAllElectricBoilersAsync_Returns_List_Of_Boilers()
    {
        //Arrange
        repoElectricBoiler.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<ElectricBoiler>
        {
            new ElectricBoiler
            {
                Id = 1,
                Name = "EB1",
                MaxHeat= 6f,
                MaxElectricity = 6f,
                ProductionCost = 15,

            },
        });
        
        var service = new ProductionUnitService(repoGasBoiler.Object, repoOilBoiler.Object, repoElectricBoiler.Object, repoGasMotor.Object, repoMaintenance.Object );
        
        //Act
        var result = await service.GetAllElectricBoilersAsync();
        
        //Assert
        Assert.NotEmpty(result);
        Assert.IsType<List<ElectricBoiler>>(result);
        Assert.Equal(1, result[0].Id);
        Assert.Equal("EB1", result[0].Name);
    }

    [Fact]
    public async Task GetAllOilBoilersAsync_Returns_List_Of_Boilers()
    {
        //Arrange
        repoOilBoiler.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<OilBoiler>
        {
            new OilBoiler
            {
                Id = 1,
                Name = "OB1",
                MaxHeat = 6.0f,
                ProductionCost = 690,
                Co2Emissions = 147,
                OilConsumption = 1.05f,
            },
        });
        
        var service = new ProductionUnitService(repoGasBoiler.Object, repoOilBoiler.Object,  repoElectricBoiler.Object, repoGasMotor.Object, repoMaintenance.Object);
        
        //Act
        var result = await service.GetAllOilBoilersAsync();
        
        //Assert
        Assert.NotEmpty(result);
        Assert.IsType<List<OilBoiler>>(result);
        Assert.Equal(1, result[0].Id);
    }

    [Fact]
    public async Task GetActiveGasBoilersAsync_Filters_Units_In_Maintenance_At_StartPeriod()
    {
        // Arrange
        DateTime startPeriod = new DateTime(2026, 01, 01, 10, 00, 00, DateTimeKind.Utc);

        repoGasBoiler.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<GasBoiler>
        {
            new GasBoiler { Id = 1, Name = "GB1" },
            new GasBoiler { Id = 2, Name = "GB2" },
        });

        repoMaintenance.Setup(x => x.GetAllMaintenancePeriodsAsync()).ReturnsAsync(new List<MaintenancePeriodPersistence>
        {
            new MaintenancePeriodPersistence
            {
                Id = 10,
                StartTime = startPeriod.AddHours(0),
                EndTime = startPeriod.AddHours(1)
            },
        });

        repoMaintenance.Setup(x => x.GetAllProductionUnitMaintenanceAsync()).ReturnsAsync(new List<ProductionUnitMaintenancePersistence>
        {
            new ProductionUnitMaintenancePersistence
            {
                Id = 100,
                MaintenancePeriodId = 10,
                UnitTypeId = 1,
                UnitId = 1
            },
        });

        var service = new ProductionUnitService(repoGasBoiler.Object, repoOilBoiler.Object,  repoElectricBoiler.Object, repoGasMotor.Object, repoMaintenance.Object);

        // Act
        List<GasBoiler> result = await service.GetActiveGasBoilersAsync(startPeriod);

        // Assert
        Assert.Single(result);
        Assert.Equal(2, result[0].Id);
    }
}