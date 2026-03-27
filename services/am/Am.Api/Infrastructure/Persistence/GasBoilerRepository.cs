using Am.Api.Application.Exceptions;
using Am.Api.Application.Interfaces;
using Am.Api.Model.DTOs;
using Am.Api.Domain.Models;
using Supabase;
using Supabase.Postgrest.Responses;

namespace Am.Api.Infrastructure.Presistence;

/// <summary>
/// Repository responsible for retrieving GasBoilerPersistence data from the database
/// using the configured database client.
/// </summary>
public class GasBoilerRepository: IProductionUnitRepository<GasBoiler>
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
    public async Task<List<GasBoiler>> GetAllAsync()
    {
        try
        {
            ModeledResponse<GasBoilerPersistence> result = await _client.From<GasBoilerPersistence>().Get();
            List<GasBoilerPersistence> gasBoilersPersistence = result.Models;

            if ( gasBoilersPersistence == null || gasBoilersPersistence.Count == 0 )
            {
                throw new NoAssetsFoundException("No asset data received. Gas Boiler set empty");
            }

            List<GasBoiler> gasBoilers = new List<GasBoiler>();

            foreach (GasBoilerPersistence gasBoiler in gasBoilersPersistence)
            {
                gasBoilers.Add(ToDomain(gasBoiler));
            }

            return gasBoilers;
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
    public async  Task<GasBoiler> GetByIdAsync(int id)
    {
        try
        {
            ModeledResponse<GasBoilerPersistence> result = await _client.From<GasBoilerPersistence>().Select(obj => new object[] { obj.Id }).Get();

            GasBoilerPersistence gasBoiler = result.Model;
            //TODO - To be finished. Validation check is to be done here
            
            return ToDomain(gasBoiler);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching GasBoilerRepository: {e.GetType()} {e.Message}");
            throw;
        }
    }

    /// <summary>
    /// Turns Persistance Model into a Domain model.
    /// </summary>
    /// <param name="p">The Persistance model.</param>
    /// <returns>A domain model.</returns>
    public static GasBoiler ToDomain(GasBoilerPersistence p)
    {
        return new GasBoiler
        {
            Id = p.Id,
            Name = p.Name,
            MaxHeat = p.MaxHeat,
            ProductionCost = (int)p.ProductionCost,
            Co2Emissions = p.Co2Emissions,
            GasConsumption = p.GasConsumption,
        };
    }
}