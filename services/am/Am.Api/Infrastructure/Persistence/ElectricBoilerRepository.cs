using Am.Api.Application.Exceptions;
using Am.Api.Application.Interfaces;
using Am.Api.Model.DTOs;
using Am.Api.Domain.Models;
using Supabase;
using Supabase.Postgrest.Responses;

namespace Am.Api.Infrastructure.Presistence;

public class ElectricBoilerRepository: IProductionUnitRepository<ElectricBoiler>
{   
   private readonly DatabaseContext _context;
   private readonly Client _client;
   
    public ElectricBoilerRepository(DatabaseContext context)
    {
        _context = context;
        _client = _context.GetClient(); 
    }

    public async Task<List<ElectricBoiler>> GetAllAsync()
    {
        try
        {
            ModeledResponse<ElectricBoilerPersistence> result = await _client.From<ElectricBoilerPersistence>().Get();
            List<ElectricBoilerPersistence> electricBoilersPersistence = result.Models;

            if (electricBoilersPersistence == null || electricBoilersPersistence.Count == 0)
            {
                throw new NoAssetsFoundException("No asset data received. Electric Boiler set empty");
            }

            List<ElectricBoiler> electricBoilers = new List<ElectricBoiler>();

            foreach (ElectricBoilerPersistence electricBoiler in electricBoilersPersistence)
            {
                electricBoilers.Add(ToDomain(electricBoiler));
            }

            return electricBoilers;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching all in ElectricBoilerRepository: {e.GetType()} {e.Message}");
            throw;
        }
    }

    public async Task<ElectricBoiler> GetByIdAsync(int id)
    {
            try
            {
                ModeledResponse<ElectricBoilerPersistence> result = await _client.From<ElectricBoilerPersistence>().Select(obj => new object[] { obj.Id }).Get();

                ElectricBoilerPersistence electricBoilerPersistence = result.Model;
                //TODO - To be finished. Validation check is to be done here
            
                return ToDomain(electricBoilerPersistence);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error fetching electricBoilerRepository: {e.GetType()} {e.Message}");
                throw;
            }
    }

    /// <summary>
    /// Turns Persistance Model into a Domain model.
    /// </summary>
    /// <param name="p">The Persistance model.</param>
    /// <returns>A domain model.</returns>
    public static ElectricBoiler ToDomain(ElectricBoilerPersistence p)
    {
        return new ElectricBoiler
        {
            Id = p.Id,
            Name = p.Name,
            MaxHeat = p.MaxHeat,
            ProductionCost = (int)p.ProductionCost,
            MaxElectricity = p.MaxElectricity,
        };
    }
}