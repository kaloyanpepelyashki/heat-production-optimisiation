namespace Am.Api.Domain.Models;


/// Parent class for Production units.
n

public abstract class ProductionUnit
{
    public int Id;
    public string Name;
    public int ProductionCost;
    public float MaxHeat;
    public bool Active;
}