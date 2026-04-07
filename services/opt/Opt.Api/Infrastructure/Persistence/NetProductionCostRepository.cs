using Opt.Api.Application.Exceptions;
using Opt.Api.Application.Interfaces;
using Opt.Api.Infrastructure.Persistence.PersistenceModels;
using Supabase;
using Supabase.Postgrest.Responses;

namespace Opt.Api.Infrastructure.Persistence;

public class NetProductionCostRepository : INetProductionCostRepository
{
    private Client _client;
    private DatabaseContext _context;
    private readonly ILogger<NetProductionCostRepository> _logger;

    public NetProductionCostRepository(DatabaseContext context, ILogger<NetProductionCostRepository> logger)
    {
        try
        {
            _context = context;
            _client = context.GetClient();
            _logger = logger;
        }
        catch (DatabaseContextException e)
        {
            Console.WriteLine($"Error with NetProductionCostRepository. DatabaseContext issue {e.Message}, {e.StackTrace}");
            _logger.LogError($"Error with NetProductionCostRepository. DatabaseContext issue {e.Message}, {e.StackTrace}");

        }
        catch (Exception e)
        {
            Console.WriteLine($"Error initialising NetProductionCostRepository: {e.Message}");
            _logger.LogError($"Error initialising NetProductionCostRepository: {e.Message}");

        }
    }

    public async Task<List<NetProductionCostPersistence>> GetAllNetProductionCostAsync()
    {
        try
        {
            ModeledResponse<NetProductionCostPersistence> result = await _client.From<NetProductionCostPersistence>().Get();
            List<NetProductionCostPersistence> netProductionCost = result.Models;
            _logger.LogInformation($"Request GetAllNetProductionCost. Returned {netProductionCost}");
            if (netProductionCost == null || netProductionCost.Count == 0)
            {
                throw new NoDataFoundException("No data found when Getting all source data");
            }

            return netProductionCost;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching all NetProductionCostPersistence: {e.GetType()} {e.Message}");
            _logger.LogError($"Error fetching all NetProductionCostPersistence: {e.GetType()} {e.Message}");
            throw;
        }
    }
}