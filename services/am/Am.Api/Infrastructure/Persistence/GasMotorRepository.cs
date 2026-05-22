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
    private readonly ILogger<GasMotorRepository> _logger;

    public GasMotorRepository(DatabaseContext context, ILogger<GasMotorRepository> logger)
    {
        _context = context;
        _client = _context.GetClient();
        _logger = logger;
    }

    public async Task<List<GasMotor>> GetAllAsync()
    {
        try
        {
            ModeledResponse<GasMotorPersistence> result = await _client.From<GasMotorPersistence>().Get();
            List<GasMotorPersistence> gasMotorsPersistence = result.Models;
            _logger.LogInformation($"Request GetAllAsync for GasMotors. Returned:  {gasMotorsPersistence}");
            

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
            Console.WriteLine($"Error fetching all in GasMotorRepository: {e.GetType()} {e.Message}");
            _logger.LogError($"Error fetching all in GasMotorRepository: {e.GetType()} {e.Message}");
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
            _logger.LogInformation($"Request GetByIdAsync for GasMotor: {gasMotorPersistence}");

            return ToDomain(gasMotorPersistence);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching specific item {id} in GasMotorRepository: {e.GetType()} {e.Message}");
            _logger.LogError($"Error fetching specific item {id} in GasMotorRepository: {e.GetType()} {e.Message}");
            throw;
        }
    }

    
    /// Turns Persistance Model into a Domain model.
    
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