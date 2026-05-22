using Supabase.Postgrest.Attributes;

namespace Am.Api.Model.DTOs;


/// Represents the Gas Boiler Entity 

[Table("gas_boilers")]
public class GasBoilerPersistence: ProductionUnitGeneralPersistence
{
    [Column("co2_emissions")]
    public int Co2Emissions { get; set; }
    
    [Column("gas_consumption")]
    public float GasConsumption { get; set; }
}