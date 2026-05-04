using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Rdm.Api.Inrastructure.Persistence.PersistenceModels;


/// <summary>
/// Stores information about the hourly optimisation of each optimisation run.
/// Has information about the heat production, electricity consumption, co2 emissions produced by the units during the optimisation run, expenses and the time frame of the schedule (hour from and to).
/// Also stores information about the production units used during the hour. 
/// </summary>
[Table("optimisation_results_hourly")]
public class OptimisationResultsHourlyPersistence: BaseModel
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