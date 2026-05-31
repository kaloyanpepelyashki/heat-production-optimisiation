namespace Am.Api.Model.DTOs;

using Supabase.Postgrest.Attributes;

[Table("oil_boilers")]
public class OilBoilerPersistence : ProductionUnitGeneralPersistence
{
    [Column("co2_emissions")]
    public int Co2Emissions { get; set; }

    [Column("oil_consumption")]
    public float OilConsumption { get; set; }
}