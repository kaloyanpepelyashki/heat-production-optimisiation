using Opt.Api.Application.Interfaces;
using Opt.Api.Infrastructure.Persistence.PersistenceModels;

namespace Opt.Api.Application.Services;

public class NetProductionCostService: INetProductionCostService
{
    private readonly INetProductionCostRepository _netProductionCostRepository;

    public NetProductionCostService(INetProductionCostRepository netProductionCostRepository)
    {
        _netProductionCostRepository = netProductionCostRepository;
    }

    public async Task<List<NetProductionCostPersistence>> GetAllNetProductionCostAsync()
    {
        try
        {
            var netProductionCost = await _netProductionCostRepository.GetAllNetProductionCostAsync();
            return netProductionCost;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in NetPrductionCostService.GetNetPrductionCost: {e.Message}, {e.GetType()}");
            throw;
        }
    }
}