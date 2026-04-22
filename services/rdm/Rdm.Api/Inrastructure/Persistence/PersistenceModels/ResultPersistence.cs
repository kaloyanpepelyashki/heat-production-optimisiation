using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Rdm.Api.Inrastructure.Persistence.PersistenceModels;

[Table("optimisation_results")]
public class ResultPersistence : BaseModel
{
  [PrimaryKey("Id")]
  public int Id { get; set; }
  [Column("heat_production")]
  public double HeatProduction { get; set; }
  [Column("electricity_consumption")]
  public double ElectricityConsumption { get; set; }
  [Column("expenses")]
  public double Expenses { get; set; }
  [Column("profit")]
  public double Profit { get; set; }
  [Column("produced_co2_emissions")]
  public double ProducedCo2Emissions { get; set; }
  [Column("date_run")]
  public DateTime DateRun { get; set; }
  
  public List<OptimisationProductionUnitsJointPersistence> optimisationProductionUnits { get; set; }
}