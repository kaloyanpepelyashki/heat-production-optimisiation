namespace Rdm.Api.Inrastructure.Persistence.PersistenceModels;

using Supabase.Postgrest.Attributes;

public class OptimisationResultHourlyWithProductionUnitPersistence : OptimisationResultsHourlyPersistence
{
    [Reference(typeof(OptimisationProductionUnitPersistence))]
    [Column("ProductionUnits")]
    public List<OptimisationProductionUnitPersistence> ProductionUnits { get; set; }
}