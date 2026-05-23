namespace Am.Api.Model.DTOs;

using Supabase.Postgrest.Attributes;

/// <summary>
/// Represents the Gas Boiler Entity 
/// </summary>
[Table("gas_boilers")]
public class GasBoilerPersistence: ProductionUnitGeneralPersistence
{
    [Column("co2_emissions")]
    public int Co2Emissions { get; set; }

    [Column("gas_consumption")]
    public float GasConsumption { get; set; }
}