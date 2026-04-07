using Opt.Api.Application.Exceptions;
using Opt.Api.Application.Interfaces;
using Opt.Api.Infrastructure.Persistence.PersistenceModels;
using Supabase;
using Supabase.Postgrest.Responses;

namespace Opt.Api.Infrastructure.Persistence;

public class SelectedProductionUnitsRepository : ISelectedProductionUnitsRepository
{
    private Client _client; 
    private DatabaseContext _context;
    private readonly ILogger<SelectedProductionUnitsRepository> _logger;

    public SelectedProductionUnitsRepository(DatabaseContext context, ILogger<SelectedProductionUnitsRepository> logger)
    {
        try
        {
            _context = context;
            _client = context.GetClient();
            _logger = logger;
        }
        catch (DatabaseContextException e)
        {
            Console.WriteLine($"Error with SelectedProductionUnitsRepository. DatabaseContext issue {e.Message}, {e.StackTrace}");
            _logger.LogError($"Error with SelectedProductionUnitsRepository. DatabaseContext issue {e.Message}, {e.StackTrace}");
            
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error initialising SelectedProductionUnitsRepository: {e.Message}");
            _logger.LogError($"Error initialising SelectedProductionUnitsRepository: {e.Message}");
            
        }
    }

    public async Task<List<SelectedProductionUnitsPersistence>> GetAllSelectedProductionUnits()
    {
        try
        {
            ModeledResponse<SelectedProductionUnitsPersistence> result = await _client.From<SelectedProductionUnitsPersistence>().Get();
            List<SelectedProductionUnitsPersistence> netProductionCost = result.Models;
            _logger.LogInformation($"Request GetAllSelectedProductionUnits. Returned {netProductionCost}");
            if (netProductionCost == null || netProductionCost.Count == 0)
            {
                throw new NoDataFoundException("No data found when Getting all source data");
            }
            
            return netProductionCost;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching all SelectedProductionUnitsPersistence: {e.GetType()} {e.Message}");
            _logger.LogError($"Error fetching all SelectedProductionUnitsPersistence: {e.GetType()} {e.Message}");
            throw;
        }
    }
}