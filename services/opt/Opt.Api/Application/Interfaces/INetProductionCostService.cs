using Opt.Api.Infrastructure.Persistence.PersistenceModels;

namespace Opt.Api.Application.Interfaces;

public interface INetProductionCostService
{
    Task<List<NetProductionCostPersistence>> GetAllNetProductionCostAsync();
}