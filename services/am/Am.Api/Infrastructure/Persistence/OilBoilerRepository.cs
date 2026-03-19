using Am.Api.Application.Exceptions;
using Am.Api.Application.Interfaces;
using Am.Api.Model.DTOs;
using Supabase;
using Supabase.Postgrest.Responses;

namespace Am.Api.Infrastructure.Presistence;

public class OilBoilerRepository : IProductionUnitRepository<OilBoilerPersistence>
{
    private readonly DatabaseContext _context;
    private readonly Client _client; 
    

    public OilBoilerRepository(DatabaseContext context)
    {
        _context = context;
        _client = context.GetClient();
    }
    
    /// <summary>
    /// Retrieves all oil boiler records from the database.
    /// Throws an exception if no data is returned.
    /// </summary>
    /// <returns>A list of OilBoilerPersistence entities (should be domain model that is returned).</returns>
    //TODO It should return a domain model, not a persistence/infrastructure model
    public async Task<List<OilBoilerPersistence>> GetAllAsync()
    {
        try
        {
            ModeledResponse<OilBoilerPersistence> result = await _client.From<OilBoilerPersistence>().Get();
            List<OilBoilerPersistence> oilBoilers = result.Models;

            if (oilBoilers == null || oilBoilers.Count == 0 )
            {
                throw new NoAssetsFoundException("No asset data received. Gas Boiler set empty");
            }

            return oilBoilers;
       }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching all in GasBoilerRepository: {e.GetType()} {e.Message}");
            throw;
        }
    }
    /// <summary>
    /// Retrieves an oil boiler record by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the gas boiler.</param>
    /// <returns>A OilBoilerPersistence entity matching the given id.</returns>
    //TODO It should return a domain model, not a persistence/infrastructure model
    public async  Task<OilBoilerPersistence> GetByIdAsync(int id)
    {
        try
        {
            ModeledResponse<OilBoilerPersistence> result = await _client.From<OilBoilerPersistence>().Select(obj => new object[] { obj.Id }).Get();

            OilBoilerPersistence oilBoiler = result.Model;
            //TODO - Should do mapping to a domain model here (not persistence model, but a domain model)
            //TODO - To be finished. Validation check is to be done here
            
            return oilBoiler;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching GasBoilerRepository: {e.GetType()} {e.Message}");
            throw;
        }
    }
}