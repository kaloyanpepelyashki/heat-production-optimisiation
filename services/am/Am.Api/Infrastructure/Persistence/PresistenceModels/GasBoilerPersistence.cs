namespace Am.Api.Model.DTOs;

using Supabase.Postgrest.Attributes;

[Table("gas_boilers")]
public class GasBoilerPersistence: ProductionUnitGeneralPersistence
{
    [Column("co2_emissions")]
    public int Co2Emissions { get; set; }

    [Column("gas_consumption")]
    public float GasConsumption { get; set; }
}