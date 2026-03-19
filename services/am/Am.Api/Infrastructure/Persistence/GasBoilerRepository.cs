using Am.Api.Application.Exceptions;
using Am.Api.Application.Interfaces;
using Am.Api.Model.DTOs;
using Supabase;
using Supabase.Postgrest.Responses;

namespace Am.Api.Infrastructure.Presistence;

/// <summary>
/// Repository responsible for retrieving GasBoilerPersistence data from the database
/// using the configured database client.
/// </summary>
public class GasBoilerRepository: IProductionUnitRepository<GasBoilerPersistence>
{
    private readonly DatabaseContext _context;
    private readonly Client _client; 
    

    public GasBoilerRepository(DatabaseContext context)
    {
        _context = context;
        _client = _context.GetClient();
    }
    
    /// <summary>
    /// Retrieves all gas boiler records from the database.
    /// Throws an exception if no data is returned.
    /// </summary>
    /// <returns>A list of GasBoilerPersistence entities.</returns>
    //TODO It should return a domain model, not a persistence/infrastructure model
    public async Task<List<GasBoilerPersistence>> GetAllAsync()
    {
        try
        {
            ModeledResponse<GasBoilerPersistence> result = await _client.From<GasBoilerPersistence>().Get();
            List<GasBoilerPersistence> gasBoilersPersistenceList = result.Models;

            if ( gasBoilersPersistenceList == null || gasBoilersPersistenceList.Count == 0 )
            {
                throw new NoAssetsFoundException("No asset data received. Gas Boiler set empty");
            }

            return gasBoilersPersistenceList;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching all in GasBoilerRepository: {e.GetType()} {e.Message}");
            throw;
        }
    }
    /// <summary>
    /// Retrieves a gas boiler record by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the gas boiler.</param>
    /// <returns>A GasBoilerPersistence entity matching the given id.</returns>
    //TODO It should return a domain model, not a persistence/infrastructure model
    public async  Task<GasBoilerPersistence> GetByIdAsync(int id)
    {
        try
        {
            ModeledResponse<GasBoilerPersistence> result = await _client.From<GasBoilerPersistence>().Select(obj => new object[] { obj.Id }).Get();

            GasBoilerPersistence gasBoiler = result.Model;
            //TODO - Should do mapping to a domain model here (not persistence model, but a domain model)
            //TODO - To be finished. Validation check is to be done here
            
            return gasBoiler;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching GasBoilerRepository: {e.GetType()} {e.Message}");
            throw;
        }
    }
}