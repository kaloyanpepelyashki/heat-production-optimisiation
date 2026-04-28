using Rdm.Api.Application.Exceptions;
using Rdm.Api.Application.Interfaces;
using Rdm.Api.Inrastructure.Persistence.PersistenceModels;
using Supabase.Postgrest;
using Client = Supabase.Client;

namespace Rdm.Api.Inrastructure.Persistence;

/// <summary>
/// Takes care of all tasks related to the optimisation result object. Countains methods for retreival and mutation of the object
/// The class makes use of a DataBase context injected via DI
/// </summary>
public class ResultRepository : IResultRepository
{
    private readonly Client _client;
    private readonly IDatabaseContext<Client> _databaseContext;
    
    private readonly ILogger<ResultRepository> _logger;

    public ResultRepository(IDatabaseContext<Client> databaseContext, ILogger<ResultRepository> logger)
    {
        _databaseContext = databaseContext;
        _client = _databaseContext.GetClient();
        _logger = logger;
    }

    /// <summary>
    /// Returns all optimisation results records.
    /// The method performs a join, joining each of the rows in optimisation_results table and the optimisation_production_units table rows belonging to the specific optimisation result. 
    /// </summary>
    /// <returns> List of OptimisationRunPersistence, each counatining the optimisation result itself and the production units belonging to it </returns>
    /// <exception cref="DatabaseOperationException">Will throw an exception if the database query fails</exception>
    public async Task<List<OptimisationRunPersistence>> GetAllOptimisationResults()
    {
        try
        {
            var databaseResponse = await _client.From<OptimisationRunPersistence>()
                .Select("*, optimisation_results_hourly(*, optimisation_production_units(*))")
                .Get();
            List<OptimisationRunPersistence> results = databaseResponse.Models;

            return results;
        }
        catch (Exception e)
        {
            _logger.LogError($"Error in ResultRepository. Error getting all optimisation results: {e.Message}");
            Console.WriteLine(e);
            throw new DatabaseOperationException($"Error getting all optimisation results. {e.Message}", e);
        }
    }
    
    /// <summary>
    /// Returns the most recent optimisation result entry
    /// Finds the optimisation run, that is less or equal to the current date, orders it in descending order and gets only one result.
    /// Joins the optimisation_result table with the optimisation_production_units table.
    /// </summary>
    /// <returns> ResultPersistence containing the optimisation result itself and the production units belonging to it</returns>
    public async Task<OptimisationRunPersistence> GetLatestOptimisationResult()
    {
        try
        {
            var currentDate = DateTime.Now;
            var databaseResponse = await _client.From<OptimisationRunPersistence>()
                                       .Filter("created_at", Constants.Operator.LessThanOrEqual, currentDate)
                                       .Order("created_at", Constants.Ordering.Descending)
                                       .Limit(1)
                                       .Select("*, optimisation_results_hourly(*, optimisation_production_units(*))")
                                       .Get();

            OptimisationRunPersistence result = databaseResponse.Model;
            
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError($"Error in ResultRepository. Error getting latest optimisation result: {e.Message}");
            throw new DatabaseOperationException($"Error getting latest optimisation result. {e.Message}", e);
        }
    }
    
    
    /// <summary>
    /// Takes care of inserting an optimisation entry to the database. First writes the OptimisationRun entry, in case the write operation is a success, populates the Hourly schedule entry and the production units entry.
    /// In case the first insert operation is a fail, throws an error. 
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    /// <exception cref="DatabaseOperationException"></exception>
    public async Task<bool> SaveOptimisationResult(OptimisationRunPersistence result)
    {
        try
        {
            var runInsertResponse = await _client
                .From<OptimisationRunPersistence>()
                .Insert(result);

            var insertedRun = runInsertResponse.Models.First();

            if (insertedRun != null)
            {
                throw new DatabaseOperationException("Error in ResultRepository. Failed to write optimisation run to database.");
            }
            
            
            // TODO Trnasaction security has to be implemented - if one insert fails, the whole trail has to be deleted. 
            foreach (var hourly in result.OptimisationResultsHourly)
            {
                hourly.OptimisationRunId = insertedRun.Id;

                var hourlyResponse = await _client
                    .From<OptimisationResultsHourlyPersistence>()
                    .Insert(hourly);

                var insertedHourly = hourlyResponse.Models.First();
                
                foreach (var unit in hourly.ProductionUnits)
                {
                    unit.OptimisationRunHourlyId = insertedHourly.Id;

                    await _client
                        .From<OptimisationProductionUnitPersistence>()
                        .Insert(unit);
                }
            }
            
            return true;
        }
        catch (Exception e)
        {
            throw new DatabaseOperationException($"Error inserting optimisation result. {e.Message}", e);
        }
    }

}