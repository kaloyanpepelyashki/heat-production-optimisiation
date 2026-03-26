using Am.Api.Application.Services;
using Am.Api.Domain.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Am.Api.Tests;

public class AssetManager_Test
{
    private ElectricBoiler electricBoiler = new ElectricBoiler()
    {
            Id = 1,
            Name = "EB1",
            MaxHeat = 3.0f,
            ProductionCost = 510,
            Active = true,
            MaxElectricity = 500.0,
    };

    private OilBoiler oilBoiler = new OilBoiler()
    {
            Id = 2,
            Name = "OB1",
            MaxHeat = 3.0f,
            ProductionCost = 510,
            Active = true,
            CO2Emission = 10,
            OilConsumption = 100.0,
    };

    private GasBoiler gasBoiler = new GasBoiler()
    {
            Id = 3,
            Name = "GB1",
            MaxHeat = 3.0f,
            ProductionCost = 510,
            Active = true,
            CO2Emission = 10,
            GasConsumption = 100.0,
    };

    private GasMotor gasMotor = new GasMotor()
    {
            Id = 4,
            Name = "GM1",
            MaxHeat = 3.0f,
            ProductionCost = 510,
            Active = true,
            MaxElectricity = 500.0,
            CO2Emission = 10,
            GasConsumption = 100.0,
    };

    private HeatingGrid heatingGrid = new HeatingGrid()
    {
        Architecture = "IDk",
        Size = 1600,
        City = "Sonderborg",
        ImageFilePath = "file",
    };

    [Fact]
    public async Task GetAllProductionUNits_Returns_List_Of_Boilers()
    {
        AssetManager assetManager = new AssetManager();
        assetManager.AddProductionUnit(electricBoiler);
        assetManager.AddProductionUnit(gasBoiler);
        assetManager.AddProductionUnit(oilBoiler);
        List<ProductionUnit> units = assetManager.GetAllUnits();

        Assert.NotEmpty(units);
        Assert.Equal(1, units[0].Id);
        Assert.Equal("GB1", units[1].Name);
        Assert.Equal(3.0f, units[2].MaxHeat);
    }

    [Fact]
    public async Task GetUnitById_Returns_ProductionUnit()
    {
        AssetManager assetManager = new AssetManager();
        assetManager.AddProductionUnit(oilBoiler);
        assetManager.AddProductionUnit(electricBoiler);

        Assert.Equal(oilBoiler, assetManager.GetUnitById(2));
        Assert.Equal("EB1", assetManager.GetUnitById(1).Name);
    }

    [Fact]
    public async Task GetUnitByName_Returns_ProductionUnit()
    {
        AssetManager assetManager = new AssetManager();
        assetManager.AddProductionUnit(gasBoiler);
        assetManager.AddProductionUnit(electricBoiler);

        Assert.Equal(electricBoiler, assetManager.GetUnitByName("EB1"));
        Assert.Equal("GB1", assetManager.GetUnitByName("GB1").Name);
    }

    [Fact]
    public async Task TurnUnitOndAndOff()
    {
        AssetManager assetManager = new AssetManager();
        assetManager.AddProductionUnit(gasBoiler);
        assetManager.AddProductionUnit(electricBoiler);
        assetManager.TurnUnitOff(assetManager.GetUnitByName("EB1"));

        Assert.True(assetManager.GetUnitByName("GB1").Active);
        Assert.False(assetManager.GetUnitByName("EB1").Active);
    }

    [Fact]
    public async Task GetHeatingGrid()
    {
        AssetManager assetManager = new AssetManager();
        assetManager.AddHeatingGrid(heatingGrid);

        Assert.Equal(heatingGrid, assetManager.GetHeatingGrid());
    }
}