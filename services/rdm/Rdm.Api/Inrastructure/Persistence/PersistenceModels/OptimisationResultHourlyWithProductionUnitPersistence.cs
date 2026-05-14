using Supabase.Postgrest.Attributes;

namespace Rdm.Api.Inrastructure.Persistence.PersistenceModels;

public class OptimisationResultHourlyWithProductionUnitPersistence: OptimisationResultsHourlyPersistence
{
    [Reference(typeof(OptimisationProductionUnitPersistence))]
    [Column("ProductionUnits")]
    public List<OptimisationProductionUnitPersistence> ProductionUnits { get; set; }
}