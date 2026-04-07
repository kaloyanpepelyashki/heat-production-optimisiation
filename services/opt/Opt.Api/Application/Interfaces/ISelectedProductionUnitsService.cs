using Opt.Api.Infrastructure.Persistence.PersistenceModels;

namespace Opt.Api.Application.Interfaces;

public interface ISelectedProductionUnitsService
{
    Task<List<SelectedProductionUnitsPersistence>> GetAllSelectedProductionUnitsAsync();
}