using Opt.Api.Infrastructure.Persistence.PersistenceModels;

namespace Opt.Api.Application.Interfaces;

public interface ISelectedProductionUnitsRepository
{
    Task<List<SelectedProductionUnitsPersistence>> GetAllSelectedProductionUnitsAsync();
}