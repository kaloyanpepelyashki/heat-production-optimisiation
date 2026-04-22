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
    private readonly DatabaseContext _databaseContext;
    
    private readonly ILogger<ResultRepository> _logger;

    public ResultRepository(IDatabaseContext<Client> _databaseContext, ILogger<ResultRepository> logger)
    {
        _databaseContext = _databaseContext;
        _client = _databaseContext.GetClient();
        _logger = logger;
    }

    /// <summary>
    /// Returns all optimisation results records.
    /// The method performs a join, joining each of the rows in optimisation_results table and the optimisation_production_units table rows belonging to the specific optimisation result. 
    /// </summary>
    /// <returns> List of ResultPersistence, each counatining the optimisation result itself and the production units belonging to it </returns>
    /// <exception cref="DatabaseOperationException">Will throw an exception if the database query fails</exception>
    public async Task<List<ResultPersistence>> GetAllOptimisationResults()
    {
        try
        {
            var databaseResponse = await _client.From<ResultPersistence>().Select("*, optimisation:optimisation_id(*, optimisation_production_units(*))").Get();
            List<ResultPersistence> results = databaseResponse.Models;

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
    public async Task<ResultPersistence> GetLatestOptimisationResult()
    {
        try
        {
            var currentDate = DateTime.Now;
            var databaseResponse = await _client.From<ResultPersistence>()
                                       .Filter("date_run", Constants.Operator.LessThanOrEqual, currentDate)
                                       .Order("date_run", Constants.Ordering.Descending)
                                       .Limit(1)
                                       .Select("*, optimisation:optimisation_id(*, optimisation_production_units(*))")
                                       .Get();

            ResultPersistence result = databaseResponse.Model;
            
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError($"Error in ResultRepository. Error getting latest optimisation result: {e.Message}");
            throw new DatabaseOperationException($"Error getting latest optimisation result. {e.Message}", e);
        }
    }
    
    
    //TODO - Has to be changed, after the Database schema is updated. 
    public async Task<ResultPersistence> SaveOptimisationResult(ResultPersistence result)
    {
        try
        {
            var inserionResult = await _client.From<ResultPersistence>().Insert(result);
            var dataBaseResponse = inserionResult.Model;

            if (dataBaseResponse != null)
            {
                throw new DatabaseOperationException("Error inserting optimisation result. ");
            }
            
            return dataBaseResponse;
            
        }
        catch (Exception e)
        {
            throw new DatabaseOperationException($"Error inserting optimisation result. {e.Message}", e);
        }
    }

}