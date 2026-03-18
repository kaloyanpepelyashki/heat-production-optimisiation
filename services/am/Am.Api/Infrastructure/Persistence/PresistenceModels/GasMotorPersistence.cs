using Supabase.Postgrest.Attributes;

namespace Am.Api.Model.DTOs;

public class GasMotorPersistence: ProductionUnitGeneralPersistence
{
    [Column("max_electricity")]
    public float MaxElectricity { get; set; }
    
    [Column("co2_emissions")]
    public int Co2Emissions { get; set; }
    
    [Column("gas_consumption")]
    public float GasConsumption { get; set; }
}