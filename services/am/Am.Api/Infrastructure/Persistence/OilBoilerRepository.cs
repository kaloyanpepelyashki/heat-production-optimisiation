namespace Am.Api.Infrastructure.Presistence;

using Am.Api.Application.Exceptions;
using Am.Api.Application.Interfaces;
using Am.Api.Domain.Models;
using Am.Api.Model.DTOs;
using Supabase;
using Supabase.Postgrest.Responses;

public class OilBoilerRepository : IProductionUnitRepository<OilBoiler>
{
    private readonly DatabaseContext _context;
    private readonly Client _client; 
    private readonly ILogger<OilBoilerRepository> _logger;

    public OilBoilerRepository(DatabaseContext context, ILogger<OilBoilerRepository> logger)
    {
        this._context = context;
        this._client = this._context.GetClient();
        this._logger = logger;
    }

    public async Task<List<OilBoiler>> GetAllAsync()
    {
        try
        {
            ModeledResponse<OilBoilerPersistence> result = await this._client.From<OilBoilerPersistence>().Get();
            List<OilBoilerPersistence> oilBoilersPersistence = result.Models;
            this._logger.LogInformation($"Request GetAllAsync for OilBoilers: {oilBoilersPersistence}");

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
            this._logger.LogError($"Error fetching all in oilBoilerRepository: {e.GetType()} {e.Message}");
            throw;
        }
    }

    public async Task<OilBoiler> GetByIdAsync(int id)
    {
        try
        {
            ModeledResponse<OilBoilerPersistence> result = await this._client.From<OilBoilerPersistence>().Select(obj => new object[] { obj.Id }).Get();

            OilBoilerPersistence oilBoiler = result.Model;
            this._logger.LogInformation($"Request GetByIdAsync for OilBoiler: {oilBoiler}");

            // TODO - To be finished. Validation check is to be done here
            return ToDomain(oilBoiler);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching oilBoilerRepository: {e.GetType()} {e.Message}");
            this._logger.LogError($"Error fetching oilBoilerRepository: {e.GetType()} {e.Message}");
            throw;
        }
    }

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