using Am.Api.Application.Exceptions;
using Am.Api.Application.Interfaces;
using Am.Api.Model.DTOs;
using Am.Api.Domain.Models;
using Supabase;
using Supabase.Postgrest.Responses;

namespace Am.Api.Infrastructure.Presistence;

public class OilBoilerRepository : IProductionUnitRepository<OilBoiler>
{
    private readonly DatabaseContext _context;
    private readonly Client _client; 
    private readonly ILogger<OilBoilerRepository> _logger;
    

    public OilBoilerRepository(DatabaseContext context, ILogger<OilBoilerRepository> logger)
    {
        _context = context;
        _client = context.GetClient();
        _logger = logger;
    }
    
    /// <summary>
    /// Retrieves all oil boiler records from the database.
    /// Throws an exception if no data is returned.
    /// </summary>
    /// <returns>A list of OilBoiler entities (should be domain model that is returned).</returns>
    public async Task<List<OilBoiler>> GetAllAsync()
    {
        try
        {
            ModeledResponse<OilBoilerPersistence> result = await _client.From<OilBoilerPersistence>().Get();
            List<OilBoilerPersistence> oilBoilersPersistence = result.Models;
            _logger.LogInformation($"Request GetAllAsync for OilBoilers: {oilBoilersPersistence}");

            if (oilBoilersPersistence == null || oilBoilersPersistence.Count == 0 )
            {
                throw new NoAssetsFoundException("No asset data received. Gas Boiler set empty");
            }

            List<OilBoiler> oilBoilers = new List<OilBoiler>();

            foreach (OilBoilerPersistence oilBoiler in oilBoilersPersistence)
            {
                oilBoilers.Add(ToDomain(oilBoiler));
            }

            return oilBoilers;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching all in oilBoilerRepository: {e.GetType()} {e.Message}");
            _logger.LogError($"Error fetching all in oilBoilerRepository: {e.GetType()} {e.Message}");
            throw;
        }
    }
    /// <summary>
    /// Retrieves an oil boiler record by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the gas boiler.</param>
    /// <returns>A OilBoiler entity matching the given id.</returns>
    public async  Task<OilBoiler> GetByIdAsync(int id)
    {
        try
        {
            ModeledResponse<OilBoilerPersistence> result = await _client.From<OilBoilerPersistence>().Select(obj => new object[] { obj.Id }).Get();

            OilBoilerPersistence oilBoiler = result.Model;
            _logger.LogInformation($"Request GetByIdAsync for OilBoiler: {oilBoiler}");
            //TODO - To be finished. Validation check is to be done here
            
            return ToDomain(oilBoiler);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching oilBoilerRepository: {e.GetType()} {e.Message}");
            _logger.LogError($"Error fetching oilBoilerRepository: {e.GetType()} {e.Message}");
            throw;
        }
    }

    /// <summary>
    /// Turns Persistance Model into a Domain model.
    /// </summary>
    /// <param name="p">The Persistance model.</param>
    /// <returns>A domain model.</returns>
    public static OilBoiler ToDomain(OilBoilerPersistence p)
    {
        return new OilBoiler
        {
            Id = p.Id,
            Name = p.Name,
            MaxHeat = p.MaxHeat,
            ProductionCost = (int)p.ProductionCost,
            Co2Emissions = p.Co2Emissions,
            OilConsumption = p.OilConsumption,
        };
    }
}