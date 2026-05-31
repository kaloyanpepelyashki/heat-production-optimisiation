namespace Am.Api.Model.DTOs;

using Supabase.Postgrest.Attributes;

[Table("electric_boilers")]
public class ElectricBoilerPersistence : ProductionUnitGeneralPersistence
{
    [Column("max_electricity")]
    public float MaxElectricity { get; set; }
}