using System.Linq;
using Am.Api.Domain.Models;

namespace Am.Api.Application.Services;

public class AssetManager
{
    private string[] avalibleTypes = { "GasBoiler", "OilBoiler", "GasMotor", "ElectricBoiler, ProductionUnit" };
    private List<ProductionUnit>? productionUnits = new List<ProductionUnit>();
    private HeatingGrid? heatingGrid;

    public ProductionUnit GetUnitById(int id, string type)
    {
        if (!avalibleTypes.Contains(type))
        {
            throw new Exception("Invalid Type of a unit");
        }
        return productionUnits.SingleOrDefault(productionUnit => productionUnit.Id == id && productionUnit.type == type);
    }

    public ProductionUnit GetUnitByName(string name)
    {
        if (!avalibleTypes.Contains(type))
        {
            throw new Exception("Invalid Type of a unit");
        }
        return productionUnits.SingleOrDefault(productionUnit => productionUnit.Name == name && productionUnit.type == type);
    }

    public void AddProductionUnit(ProductionUnit productionUnit)
    {
        productionUnits.Add(productionUnit);
    }

    public List<ProductionUnit> GetAllUnits()
    {
        return productionUnits;
    }

    public List<ProductionUnit> GetAllUnits(string type)
    {
        if (!avalibleTypes.Contains(type))
        {
            throw new Exception("Invalid Type of a unit");
        }
        return productionUnits.Where(productionUnit => productionUnit.type == type).ToList();
    }

    public void TurnUnitOff(ProductionUnit productionUnit)
    {
        productionUnit.Active = false;
    }

    public void TurnUnitOn(ProductionUnit productionUnit)
    {
        productionUnit.Active = false;
    }

    public HeatingGrid GetHeatingGrid()
    {
        return heatingGrid;
    }

    public void AddHeatingGrid(HeatingGrid HG)
    {
        heatingGrid = HG;
    }
}