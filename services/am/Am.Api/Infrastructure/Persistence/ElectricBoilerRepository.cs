using Am.Api.Application.Exceptions;
using Am.Api.Application.Interfaces;
using Am.Api.Model.DTOs;
using Supabase;
using Supabase.Postgrest.Responses;

namespace Am.Api.Infrastructure.Presistence;

public class ElectricBoilerRepository: IProductionUnitRepository<ElectricBoilerPersistence>
{   
   private readonly DatabaseContext _context;
   private readonly Client _client;
   
    public ElectricBoilerRepository(DatabaseContext context)
    {
        _context = context;
        _client = _context.GetClient(); 
    }

    public async Task<List<ElectricBoilerPersistence>> GetAllAsync()
    {
        try
        {
            ModeledResponse<ElectricBoilerPersistence> result = await _client.From<ElectricBoilerPersistence>().Get();
            List<ElectricBoilerPersistence> electricBoilersPersistence = result.Models;

            if (electricBoilersPersistence == null || electricBoilersPersistence.Count == 0)
            {
                throw new NoAssetsFoundException("No asset data received. Electric Boiler set empty");
            }

            return electricBoilersPersistence;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching all in ElectricBoilerRepository: {e.GetType()} {e.Message}");
            throw;
        }
    }

    public async Task<ElectricBoilerPersistence> GetByIdAsync(int id)
    {
            try
            {
                ModeledResponse<ElectricBoilerPersistence> result = await _client.From<ElectricBoilerPersistence>().Select(obj => new object[] { obj.Id }).Get();

                ElectricBoilerPersistence electricBoilerPersistence = result.Model;
                //TODO - Should do mapping to a domain model here (not persistence model, but a domain model)
                //TODO - To be finished. Validation check is to be done here
            
                return electricBoilerPersistence;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error fetching electricBoilerRepository: {e.GetType()} {e.Message}");
                throw;
            }
    }
}