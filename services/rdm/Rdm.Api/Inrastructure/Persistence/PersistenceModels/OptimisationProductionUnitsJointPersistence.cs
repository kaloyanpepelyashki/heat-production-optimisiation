using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Rdm.Api.Inrastructure.Persistence.PersistenceModels;

[Table("optimisation_production_units")]
public class OptimisationProductionUnitsJointPersistence : BaseModel
{
    [PrimaryKey("Id")]
    public int Id { get; set; }
    [Column("optimisation_id")]
    public int OptimisationId { get; set; }
    [Column("production_unit_id")]
    public int ProductionUnitId { get; set; }
    [Column("production_unit_type")]
    public string ProductionUnitType { get; set; }
}