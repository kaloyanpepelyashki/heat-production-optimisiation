using Supabase.Postgrest.Attributes;

namespace Am.Api.Model.DTOs;

[Table("electric_boilers")]
public class ElectricBoilerPersistence:   ProductionUnitGeneralPersistence
{
    [Column("max_electricity")]
    public float MaxElectricity { get; set; }
    
}