using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Rdm.Api.Inrastructure.Persistence.PersistenceModels;

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
}