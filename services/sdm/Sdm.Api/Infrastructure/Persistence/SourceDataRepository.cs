using Sdm.Api.Application.Exceptions;
using Sdm.Api.Application.Interfaces;
using Sdm.Api.Infrastructure.Persistence.PersistenceModels;
using Supabase;
using Supabase.Postgrest.Responses;

namespace Sdm.Api.Infrastructure.Persistence;

public class SourceDataRepository : ISourceDataRepository
{
    private Client _client; 
    private DatabaseContext _context;
    private readonly ILogger<SourceDataRepository> _logger;

    public SourceDataRepository(DatabaseContext context, ILogger<SourceDataRepository> logger)
    {
        try
        {
            _context = context;
            _client = context.GetClient();
        }
        catch (DatabaseContextException e)
        {
            Console.WriteLine($"Error with SourceDataRepository. DatabaseContext issue {e.Message}, {e.StackTrace}");
            _logger.LogError($"Error with SourceDataRepository. DatabaseContext issue {e.Message}, {e.StackTrace}");
            
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error initialising SourceDataRepository: {e.Message}");
            _logger.LogError($"Error initialising SourceDataRepository: {e.Message}");
            
        }
    }

    public async Task<List<SourceDataPersistence>> GetAllSourceData()
    {
        try
        {
            ModeledResponse<SourceDataPersistence> result = await _client.From<SourceDataPersistence>().Get();
            List<SourceDataPersistence> sourceData = result.Models;
            _logger.LogInformation($"Request GetAllSourceData. Returned {sourceData}");
            if (sourceData == null || sourceData.Count == 0)
            {
                throw new NoDataFoundException("No data found when Getting all source data");
            }
            
            return sourceData;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching all in GasBoilerRepository: {e.GetType()} {e.Message}");
            _logger.LogError($"Error fetching all in GasBoilerRepository: {e.GetType()} {e.Message}");
            throw;
        }
    }
}