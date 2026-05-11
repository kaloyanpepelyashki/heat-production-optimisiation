using Rdm.Api.Application.Model;

namespace Rdm.Api.Inrastructure.Persistence.PersistenceModels;

public class OptimisationResultHourlyWithProductionUnitPersistence: OptimisationResultsHourlyPersistence
{
    public List<ProductionUnit> ProductionUnits { get; set; }
}