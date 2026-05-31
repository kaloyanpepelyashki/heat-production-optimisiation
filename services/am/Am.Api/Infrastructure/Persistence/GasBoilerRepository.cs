namespace Am.Api.Infrastructure.Presistence;

using Am.Api.Application.Exceptions;
using Am.Api.Application.Interfaces;
using Am.Api.Domain.Models;
using Am.Api.Model.DTOs;
using Supabase;
using Supabase.Postgrest.Responses;

public class GasBoilerRepository : IProductionUnitRepository<GasBoiler>
{
    private readonly DatabaseContext _context;
    private readonly Client _client;
    private readonly ILogger<GasBoilerRepository> _logger;

    public GasBoilerRepository(DatabaseContext context, ILogger<GasBoilerRepository> logger)
    {
        this._context = context;
        this._client = this._context.GetClient();
        this._logger = logger;
    }

    public async Task<List<GasBoiler>> GetAllAsync()
    {
        try
        {
            ModeledResponse<GasBoilerPersistence> result = await _client.From<GasBoilerPersistence>().Get();
            List<GasBoilerPersistence> gasBoilersPersistence = result.Models;
            this._logger.LogInformation($"Request GetAllAsync for GasBoilers. Returned:  {gasBoilersPersistence}");

            if (gasBoilersPersistence == null || gasBoilersPersistence.Count == 0 )
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
            this._logger.LogError($"Error fetching all in GasBoilerRepository: {e.GetType()} {e.Message}");
            throw;
        }
    }

    public async Task<GasBoiler> GetByIdAsync(int id)
    {
        try
        {
            ModeledResponse<GasBoilerPersistence> result = await this._client.From<GasBoilerPersistence>().Select(obj => new object[] { obj.Id }).Get();

            GasBoilerPersistence gasBoiler = result.Model;
            this._logger.LogInformation($"Request GetByIdAsync for GasBoilers. Returned:  {gasBoiler}");

            // TODO - To be finished. Validation check is to be done here
            return ToDomain(gasBoiler);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching GasBoilerRepository: {e.GetType()} {e.Message}");
            this._logger.LogError($"Error fetching GasBoilerRepository: {e.GetType()} {e.Message}");
            throw;
        }
    }

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