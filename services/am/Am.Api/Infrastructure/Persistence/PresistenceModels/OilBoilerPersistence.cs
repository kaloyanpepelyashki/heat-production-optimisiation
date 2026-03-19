using Supabase.Postgrest.Attributes;

namespace Am.Api.Model.DTOs;

[Table("oil_boilers")]
public class OilBoilerPersistence: ProductionUnitGeneralPersistence
{
    [Column("co2_emissions")]
    public int Co2Emissions { get; set; }
    
    [Column("oil_consumption")]
    public float OilConsumption { get; set; }
}