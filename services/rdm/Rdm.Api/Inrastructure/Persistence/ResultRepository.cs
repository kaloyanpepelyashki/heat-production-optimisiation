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
    public async Task<List<OptimisationRunWithHourlyResultsPersistence>> GetAllOptimisationResults()
    {
        try
        {
            var databaseResponse = await _client.From<OptimisationRunWithHourlyResultsPersistence>()
                .Select("*, OptimisationResultsHourly:optimisation_results_hourly(*, ProductionUnits:optimisation_production_units(*))")
                .Get();
            List<OptimisationRunWithHourlyResultsPersistence> results = databaseResponse.Models;

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
    public async Task<bool> SaveOptimisationResult(OptimisationRunPersistenceWrapper result)
    {
        try
        {
            var runInsertResponse = await _client
                .From<OptimisationRunPersistence>()
                .Insert(result.OptimisationRunPersistence);

            OptimisationRunPersistence? insertedRun = runInsertResponse.Models.FirstOrDefault();

            if (insertedRun == null)
            {
                throw new DatabaseOperationException("Error in ResultRepository. Failed to write optimisation run to database.");
            }
            
            foreach (OptimisationResultsHourlyPersistenceWrapper hourly in result.OptimisationResultsHourlyPersistence)
            {
                
                (bool Succeess, OptimisationResultsHourlyPersistence? Hourly) hourlyCreationResult = await CreateOptimisationHourlyEntry(hourly.HourlyResult, insertedRun.Id);

                if (!hourlyCreationResult.Succeess || hourlyCreationResult.Hourly == null)
                {
                     await _client.From<OptimisationRunPersistence>().Where(x => x.Id == insertedRun.Id).Delete();
                     return false;
                }
                
                
                foreach (OptimisationProductionUnitPersistence productionUnit in hourly.ProductionUnitsPersistence)
                {
                    (bool, OptimisationProductionUnitPersistence?) productionUnitCreationResult = await CreateProductionUnitEntry(productionUnit, hourlyCreationResult.Item2.Id);

                    if (!productionUnitCreationResult.Item1)
                    {
                        await _client.From<OptimisationRunPersistence>().Where(x => x.Id == insertedRun.Id).Delete();
                        await _client.From<OptimisationResultsHourlyPersistence>().Where(x => x.Id == hourlyCreationResult.Item2.Id).Delete();
                        return false;
                    }
                }
            }
            
            return true;
        }
        catch (Exception e)
        {
            throw new DatabaseOperationException($"Error inserting optimisation result. {e.Message}", e);
        }
    }

    private async Task<(bool Success, OptimisationResultsHourlyPersistence? Hourly)> CreateOptimisationHourlyEntry(OptimisationResultsHourlyPersistence hourlyResult, int optimisationRunId)
    {
        try
        {
            hourlyResult.OptimisationRunId = optimisationRunId;

            var hourlyResponse = await _client
                .From<OptimisationResultsHourlyPersistence>()
                .Insert(hourlyResult);

            OptimisationResultsHourlyPersistence? insertedHourly =
                hourlyResponse.Models.FirstOrDefault();

            if (insertedHourly == null)
            {
                throw new DatabaseOperationException(
                    "Error in ResultRepository. Error creating result hourly entry. Failed to write optimisation hourly result to database.");
            }

            return (true, insertedHourly);
        }
        catch (DatabaseOperationException e)
        {
            _logger.LogError($"Error inserting optimisation result. {e.Message}");
            Console.WriteLine(e);
            
            return (false, null);
        }
    }

    private async Task<(bool, OptimisationProductionUnitPersistence)> CreateProductionUnitEntry(OptimisationProductionUnitPersistence productionUnit, int hourlyResultId)
    {
        try
        {
            productionUnit.OptimisationRunHourlyId = hourlyResultId;

            var productionUnitResponse = await _client
                .From<OptimisationProductionUnitPersistence>()
                .Insert(productionUnit);
            
            OptimisationProductionUnitPersistence? insertedProductionUnit = productionUnitResponse.Models.FirstOrDefault();

            if (insertedProductionUnit == null)
            {
                throw new DatabaseOperationException("Error in ResultRepository. Error creating result production unit.");
            }
            
            return (true, insertedProductionUnit);
        }
        catch (DatabaseOperationException e)
        {
            _logger.LogError($"Error inserting optimisation result. {e.Message}");
            Console.WriteLine(e);
            
            return (false, null);
        }
    }
    

}