namespace Am.Api.Domain.Models;

/// <summary>
/// Parent class for Production units.
/// </summary>n

public abstract class ProductionUnit
{
    public int? Id;
    public string? Name;
    public int? ProductionCost;
    public double? MaxHeat;
    public bool? Active;
}