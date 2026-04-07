using Opt.Api.Infrastructure.Persistence.PersistenceModels;

namespace Opt.Api.Application.Interfaces;

public interface INetProductionCostRepository
{
    Task<List<NetProductionCostPersistence>> GetAllNetProductionCostAsync();
}