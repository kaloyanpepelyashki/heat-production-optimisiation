using Am.Api.Application.Exceptions;
using Am.Api.Application.Interfaces;
using Am.Api.Model.DTOs;
using Supabase;
using Supabase.Postgrest.Responses;

namespace Am.Api.Infrastructure.Presistence;

public class GasMotorRepository : IProductionUnitRepository<GasMotorPersistence>
{
    private readonly DatabaseContext _context;
    private readonly Client _client;

    public GasMotorRepository(DatabaseContext context)
    {
        _context = context;
        _client = _context.GetClient();
    }

    public async Task<List<GasMotorPersistence>> GetAllAsync()
    {
        try
        {
            ModeledResponse<GasMotorPersistence> result = await _client.From<GasMotorPersistence>().Get();
            List<GasMotorPersistence> gasMotorsPersistence = result.Models;

            if (gasMotorsPersistence == null || gasMotorsPersistence.Count == 0)
            {
                throw new NoAssetsFoundException("No asset data received. Gas Boiler set empty");
            }
            
            return gasMotorsPersistence;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching all in GasBoilerRepository: {e.GetType()} {e.Message}");
            throw;
        }
    }

    public async Task<GasMotorPersistence> GetByIdAsync(int id)
    {
        try
        {
            ModeledResponse<GasMotorPersistence> result =
                await _client.From<GasMotorPersistence>().Select(motor => new object[] { motor.Id }).Get();

            GasMotorPersistence gasMotorPersistence = result.Model;

            return gasMotorPersistence;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching specific item {id} in GasMotorRepository: {e.GetType()} {e.Message}");
            throw;
        }
    }
}