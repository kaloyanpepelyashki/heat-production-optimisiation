namespace Rdm.Api.Inrastructure.Persistence.PersistenceModels;

using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("optimisation_results_hourly")]
public class OptimisationResultsHourlyPersistence : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }

    [Column("optimisation_run_id")]
    public int OptimisationRunId { get; set; }

    [Column("heat_production")]
    public double HeatProduction { get; set; }

    [Column("electricity_consumption")]
    public double ElectricityConsumption { get; set; }

    [Column("co2_emissions")]
    public double Co2Emissions { get; set; }

    [Column("expenses")]
    public double Expenses { get; set; }

    [Column("time_from")]
    public DateTime TimeFrom { get; set; }

    [Column("time_to")]
    public DateTime TimeTo { get; set; }
}