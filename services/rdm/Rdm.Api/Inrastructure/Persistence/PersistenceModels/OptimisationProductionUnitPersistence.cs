namespace Rdm.Api.Inrastructure.Persistence.PersistenceModels;

using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("optimisation_production_units")]
public class OptimisationProductionUnitPersistence: BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("optimisation_results_hourly_id")]
    public int OptimisationRunHourlyId { get; set; }

    [Column("production_unit_id")]
    public int ProductionUnitId { get; set; }

    [Column("production_unit_type")]
    public string ProductionUnitType { get; set; }

    [Column("capacity")]
    public double Capacity { get; set; }

    [Column("expenses")]
    public double Expenses { get; set; }

    [Column("co2Emissions")]
    public double Co2Emissions { get; set; }

    [Column("heat_production")]
    public double HeatProduction { get; set; }

    [Column("electricity_consumption")]
    public double ElectricityConsumption { get; set; }
}