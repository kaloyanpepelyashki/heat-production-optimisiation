using Am.Api.Application.Exceptions;
using Am.Api.Application.Interfaces;
using Am.Api.Model.DTOs;
using Am.Api.Domain.Models;
using Supabase;
using Supabase.Postgrest.Responses;

namespace Am.Api.Infrastructure.Presistence;

public class GasMotorRepository : IProductionUnitRepository<GasMotor>
{
    private readonly DatabaseContext _context;
    private readonly Client _client;

    public GasMotorRepository(DatabaseContext context)
    {
        _context = context;
        _client = _context.GetClient();
    }

    public async Task<List<GasMotor>> GetAllAsync()
    {
        try
        {
            ModeledResponse<GasMotorPersistence> result = await _client.From<GasMotorPersistence>().Get();
            List<GasMotorPersistence> gasMotorsPersistence = result.Models;

            if (gasMotorsPersistence == null || gasMotorsPersistence.Count == 0)
            {
                throw new NoAssetsFoundException("No asset data received. Gas Boiler set empty");
            }

            List<GasMotor> gasMotors = new List<GasMotor>();

            foreach (GasMotorPersistence gasMotor in gasMotorsPersistence)
            {
                gasMotors.Add(ToDomain(gasMotor));
            }
            return gasMotors;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching all in GasBoilerRepository: {e.GetType()} {e.Message}");
            throw;
        }
    }

    public async Task<GasMotor> GetByIdAsync(int id)
    {
        try
        {
            ModeledResponse<GasMotorPersistence> result =
                await _client.From<GasMotorPersistence>().Select(motor => new object[] { motor.Id }).Get();

            GasMotorPersistence gasMotorPersistence = result.Model;

            return ToDomain(gasMotorPersistence);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching specific item {id} in GasMotorRepository: {e.GetType()} {e.Message}");
            throw;
        }
    }

    /// <summary>
    /// Turns Persistance Model into a Domain model.
    /// </summary>
    /// <param name="p">The Persistance model.</param>
    /// <returns>A domain model.</returns>
    public static GasMotor ToDomain(GasMotorPersistence p)
    {
        return new GasMotor
        {
            Id = p.Id,
            Name = p.Name,
            MaxHeat = p.MaxHeat,
            ProductionCost = (int)p.ProductionCost,
            Co2Emissions = p.Co2Emissions,
            MaxElectricity = p.MaxElectricity,
            GasConsumption = p.GasConsumption,
        };
    }
}