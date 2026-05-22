using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Rdm.Api.Inrastructure.Persistence.PersistenceModels;


/// Represents a whole optimisation run. 

[Table("optimisation_run")]
public class OptimisationRunPersistence: BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; set; }
    [Column("time_from")]
    public DateTime TimeFrom { get; set; }
    [Column("time_to")]
    public DateTime TimeTo { get; set; }
    [Column("scenario")]
    public string Scenario { get; set; }
    [Column("period")]
    public string Type { get; set; }
}