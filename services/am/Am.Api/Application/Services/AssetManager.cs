using System.Linq;
using Am.Api.Domain.Models;

namespace Am.Api.Application.Services;

class AssetManager {
    private List<ProductionUnit>? productionUnits;
    private HeatingGrid? heatingGrid;
    public ProductionUnit GetUnitById(int id)
    {
        return productionUnits.SingleOrDefault(productionUnit => productionUnit.Id == id);
    } 

    public ProductionUnit GetUnitbyName(string name)
    {
        return productionUnits.SingleOrDefault(productionUnit => productionUnit.Name == name);
    }

    public List<ProductionUnit> GetAllUnits()
    {
        return productionUnits;
    }

    public void TurnOffUnit(ProductionUnit productionUnit)
    {
        productionUnit.Active = false;
    }

    public void TurnOnUnit(ProductionUnit productionUnit)
    {
        productionUnit.Active = false;
    }

    public HeatingGrid GetHeatingGrid()
    {
        return heatingGrid;
    }
}