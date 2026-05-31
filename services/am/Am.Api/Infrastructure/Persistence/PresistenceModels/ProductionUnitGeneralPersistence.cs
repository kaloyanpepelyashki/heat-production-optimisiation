namespace Am.Api.Model.DTOs;

using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

public class ProductionUnitGeneralPersistence : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("max_heat")]
    public float MaxHeat { get; set; }

    [Column("production_cost")]
    public float ProductionCost { get; set; }
}