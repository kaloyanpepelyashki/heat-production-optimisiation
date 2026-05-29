namespace Am.Api.Model.DTOs;

using Supabase.Postgrest.Attributes;

[Table("gas_motors")]
public class GasMotorPersistence : ProductionUnitGeneralPersistence
{
    [Column("max_electricity")]
    public float MaxElectricity { get; set; }

    [Column("co2_emissions")]
    public int Co2Emissions { get; set; }

    [Column("gas_consumption")]
    public float GasConsumption { get; set; }
}