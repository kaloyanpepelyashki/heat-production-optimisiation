using Opt.Api.Application.Interfaces;
using Opt.Api.Infrastructure.Persistence.PersistenceModels;

namespace Opt.Api.Application.Services;

public class NetProductionCostService: INetProductionCostService
{
    private readonly INetProductionCostRepository _netPrductionCostRepository;

    public NetProductionCostService(INetProductionCostRepository netPrductionCostRepository)
    {
        _netPrductionCostRepository = netPrductionCostRepository;
    }

    public async Task<List<NetProductionCostPersistence>> GetAllNetProductionCost()
    {
        try
        {
            var netPrductionCost = await _netPrductionCostRepository.GetAllNetProductionCost();
            return netPrductionCost;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in NetPrductionCostService.GetNetPrductionCost: {e.Message}, {e.GetType()}");
            throw;
        }
    }
}