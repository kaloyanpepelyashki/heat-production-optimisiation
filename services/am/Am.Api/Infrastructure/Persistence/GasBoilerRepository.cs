using Am.Api.Application.Exceptions;
using Am.Api.Application.Interfaces;
using Am.Api.Model.DTOs;
using Supabase;
using Supabase.Postgrest.Responses;

namespace Am.Api.Infrastructure.Presistence;

public class GasBoilerRepository: IProductionUnitRepository<GasBoilerPersistence>
{
    private readonly DatabaseContext _context;
    private readonly Client _client; 

    public GasBoilerRepository(DatabaseContext context)
    {
        _context = context;
        _client = context.GetClient();
    }
    
    public async Task<List<GasBoilerPersistence>> GetAllAsync()
    {
        try
        {
            ModeledResponse<GasBoilerPersistence> result = await _client.From<GasBoilerPersistence>().Get();
            List<GasBoilerPersistence> gasBoilers = result.Models;

            if (gasBoilers.Count == 0 | gasBoilers == null)
            {
                throw new NoAssetsFoundException("No asset data received. Gas Boiler set empty");
            }

            return gasBoilers;
e       }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching all in GasBoilerRepository: {e.GetType()} {e.Message}");
            throw e;
        }
    }
    
    public  Task<GasBoilerPersistence> GetByIdAsync(int id)
    {
        
    }
}