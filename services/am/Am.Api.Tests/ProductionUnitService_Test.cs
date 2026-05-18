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
        
        var result = await service.GetAllGasBoilersAsync();
        
        
        Assert.NotEmpty(result);
        Assert.Equal(1, result[0].Id);
        Assert.Equal("GB1", result[0].Name);
        Assert.Equal(3.0f, result[0].MaxHeat);
    }

    [Fact]
    public async Task GetAllGasMotorsAsync_Returns_List_Of_Motors()
    {
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
        
        var result = await service.GetAllGasMotorsAsync();
        
        Assert.IsType<List<GasMotor>>(result);
        Assert.Equal(1, result[0].Id);
        Assert.Equal("GM1", result[0].Name);
    }

    [Fact]
    public async Task GetAllElectricBoilersAsync_Returns_List_Of_Boilers()
    {
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
        
        var result = await service.GetAllElectricBoilersAsync();
        
        Assert.NotEmpty(result);
        Assert.IsType<List<ElectricBoiler>>(result);
        Assert.Equal(1, result[0].Id);
        Assert.Equal("EB1", result[0].Name);
    }

    [Fact]
    public async Task GetAllOilBoilersAsync_Returns_List_Of_Boilers()
    {
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
        
        var result = await service.GetAllOilBoilersAsync();
        
        Assert.NotEmpty(result);
        Assert.IsType<List<OilBoiler>>(result);
        Assert.Equal(1, result[0].Id);
    }

    [Fact]
    public async Task GetProductionUnitMaintenanceByIdAsync_Returns_Maintenance()
    {
        repoMaintenance.Setup(x => x.GetAllProductionUnitMaintenanceAsync()).ReturnsAsync(new List<ProductionUnitMaintenance>
        {
            new ProductionUnitMaintenance
            {
                Id = 42,
                UnitType = "gasBoiler",
                UnitId = 7,
                CreatedAt = new DateTime(2026, 01, 01, 10, 00, 00, DateTimeKind.Utc),
                FromDate = new DateTime(2026, 01, 02, 00, 00, 00, DateTimeKind.Utc),
                ToDate = new DateTime(2026, 01, 03, 00, 00, 00, DateTimeKind.Utc),
            },
        });

        var service = new ProductionUnitService(repoGasBoiler.Object, repoOilBoiler.Object, repoElectricBoiler.Object, repoGasMotor.Object, repoMaintenance.Object);

        ProductionUnitMaintenance result = await service.GetProductionUnitMaintenanceByIdAsync(42);

        Assert.Equal(42, result.Id);
        Assert.Equal("gasBoiler", result.UnitType);
        Assert.Equal(7, result.UnitId);
    }

    [Fact]
    public async Task PostProductionUnitMaintenanceAsync_Returns_New_Id()
    {
        repoMaintenance
            .Setup(x => x.PostProductionUnitMaintenanceAsync(It.IsAny<ProductionUnitMaintenance>()))
            .ReturnsAsync(123);

        var service = new ProductionUnitService(repoGasBoiler.Object, repoOilBoiler.Object, repoElectricBoiler.Object, repoGasMotor.Object, repoMaintenance.Object);

        ProductionUnitMaintenance maintenanceToPost = new ProductionUnitMaintenance
        {
            UnitType = "gasBoiler",
            UnitId = 7,
            FromDate = new DateTime(2026, 02, 01, 00, 00, 00, DateTimeKind.Utc),
            ToDate = new DateTime(2026, 02, 02, 00, 00, 00, DateTimeKind.Utc),
        };

        int result = await service.PostProductionUnitMaintenanceAsync(maintenanceToPost);

        Assert.Equal(123, result);
    }

    [Fact]
    public async Task GetAllGasBoilersAsync_ThrowsException_OnDatabaseFailure()
    {
        repoGasBoiler.Setup(x => x.GetAllAsync()).ThrowsAsync(new Exception("Database connection failed"));
        
        var service = new ProductionUnitService(repoGasBoiler.Object, repoOilBoiler.Object, repoElectricBoiler.Object, repoGasMotor.Object, repoMaintenance.Object);
        
        await Assert.ThrowsAsync<Exception>(() => service.GetAllGasBoilersAsync());
    }

    [Fact]
    public async Task GetAllGasBoilersAsync_ReturnsEmptyList_WhenNoDataExists()
    {
        repoGasBoiler.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<GasBoiler>());
        
        var service = new ProductionUnitService(repoGasBoiler.Object, repoOilBoiler.Object, repoElectricBoiler.Object, repoGasMotor.Object, repoMaintenance.Object);
        
        var result = await service.GetAllGasBoilersAsync();
        
        Assert.Empty(result);
        Assert.IsType<List<GasBoiler>>(result);
    }

    [Fact]
    public async Task GetProductionUnitMaintenanceByIdAsync_ThrowsKeyNotFoundException_WhenIdDoesNotExist()
    {
        repoMaintenance.Setup(x => x.GetAllProductionUnitMaintenanceAsync()).ReturnsAsync(new List<ProductionUnitMaintenance>
        {
            new ProductionUnitMaintenance { Id = 42 }
        });
        var service = new ProductionUnitService(repoGasBoiler.Object, repoOilBoiler.Object, repoElectricBoiler.Object, repoGasMotor.Object, repoMaintenance.Object);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetProductionUnitMaintenanceByIdAsync(99));
    }

    [Fact]
    public async Task PostProductionUnitMaintenanceAsync_ThrowsArgumentException_WhenDatesAreInvalid()
    {
        var service = new ProductionUnitService(repoGasBoiler.Object, repoOilBoiler.Object, repoElectricBoiler.Object, repoGasMotor.Object, repoMaintenance.Object);

        var maintenanceToPost = new ProductionUnitMaintenance
        {
            UnitType = "gasBoiler",
            UnitId = 7,
            FromDate = new DateTime(2026, 03, 01, 00, 00, 00, DateTimeKind.Utc),
            ToDate = new DateTime(2026, 02, 01, 00, 00, 00, DateTimeKind.Utc),
        };

        repoMaintenance.Setup(x => x.PostProductionUnitMaintenanceAsync(It.IsAny<ProductionUnitMaintenance>()))
            .ThrowsAsync(new ArgumentException("ToDate must be after FromDate"));

        await Assert.ThrowsAsync<ArgumentException>(() => service.PostProductionUnitMaintenanceAsync(maintenanceToPost));
    }
}