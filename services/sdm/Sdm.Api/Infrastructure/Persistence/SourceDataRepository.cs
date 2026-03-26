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

    public SourceDataRepository(DatabaseContext context)
    {
        try
        {
            _context = context;
            _client = context.GetClient();
        }
        catch (DatabaseContextException e)
        {
            Console.WriteLine($"Error with SourceDataRepository. {e.Message}, {e.StackTrace}");
            
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error initialising SourceDataRepository: {e.Message}");
            
        }
    }

    public async Task<List<SourceDataPersistence>> GetAllSourceData()
    {
        try
        {
            ModeledResponse<SourceDataPersistence> result = await _client.From<SourceDataPersistence>().Get();
            List<SourceDataPersistence> sourceData = result.Models;

            if (sourceData == null || sourceData.Count == 0)
            {
                throw new NoDataFoundException("No data found when Getting all source data");
            }
            
            return sourceData;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching all in GasBoilerRepository: {e.GetType()} {e.Message}");
            throw;
        }
    }
}