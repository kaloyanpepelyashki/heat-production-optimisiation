namespace Rdm.Api.Inrastructure.Persistence;

using Rdm.Api.Application.Exceptions;
using Rdm.Api.Application.Interfaces;
using Rdm.Api.Inrastructure.Persistence.PersistenceModels;
using Supabase.Postgrest;
using Client = Supabase.Client;

public class ResultRepository : IResultRepository
{
    private readonly Client _client;
    private readonly IDatabaseContext<Client> _databaseContext;
    private readonly ILogger<ResultRepository> _logger;

    public ResultRepository(IDatabaseContext<Client> databaseContext, ILogger<ResultRepository> logger)
    {
        this._databaseContext = databaseContext;
        this._client = this._databaseContext.GetClient();
        this._logger = logger;
    }

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
            this._logger.LogError($"Error in ResultRepository. Error getting all optimisation results: {e.Message}");
            Console.WriteLine(e);
            throw new DatabaseOperationException($"Error getting all optimisation results. {e.Message}", e);
        }
    }

    public async Task<OptimisationRunPersistence> GetLatestOptimisationResult()
    {
        try
        {
            var currentDate = DateTime.Now;
            var databaseResponse = await this._client.From<OptimisationRunPersistence>()
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
            this._logger.LogError($"Error in ResultRepository. Error getting latest optimisation result: {e.Message}");
            throw new DatabaseOperationException($"Error getting latest optimisation result. {e.Message}", e);
        }
    }

    public async Task<bool> SaveOptimisationResult(OptimisationRunPersistenceWrapper result)
    {
        try
        {
            var runInsertResponse = await this._client
                .From<OptimisationRunPersistence>()
                .Insert(result.OptimisationRunPersistence);

            OptimisationRunPersistence? insertedRun = runInsertResponse.Models.FirstOrDefault();

            if (insertedRun == null)
            {
                throw new DatabaseOperationException("Error in ResultRepository. Failed to write optimisation run to database.");
            }

            foreach (OptimisationResultsHourlyPersistenceWrapper hourly in result.OptimisationResultsHourlyPersistence)
            {
                (bool Succeess, OptimisationResultsHourlyPersistence? Hourly) hourlyCreationResult = await this.CreateOptimisationHourlyEntry(hourly.HourlyResult, insertedRun.Id);

                if (!hourlyCreationResult.Succeess || hourlyCreationResult.Hourly == null)
                {
                     await this._client.From<OptimisationRunPersistence>().Where(x => x.Id == insertedRun.Id).Delete();
                     return false;
                }

                (bool, OptimisationProductionUnitPersistence?) productionUnitCreationResult = await this.CreateProductionUnitEntry(hourly.ProductionUnitsPersistence, hourlyCreationResult.Item2.Id);

                if (!productionUnitCreationResult.Item1)
                {
                    await this._client.From<OptimisationRunPersistence>().Where(x => x.Id == insertedRun.Id).Delete();
                    await this._client.From<OptimisationResultsHourlyPersistence>().Where(x => x.Id == hourlyCreationResult.Item2.Id).Delete();
                    return false;
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

            var hourlyResponse = await this._client
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
            this._logger.LogError($"Error inserting optimisation result. {e.Message}");
            Console.WriteLine(e);

            return (false, null);
        }
    }

    private async Task<(bool, OptimisationProductionUnitPersistence)> CreateProductionUnitEntry(List<OptimisationProductionUnitPersistence> productionUnits, int hourlyResultId)
    {
        try
        {
            foreach (var productionUnit in productionUnits)
            {
                productionUnit.OptimisationRunHourlyId = hourlyResultId;
            }

            var productionUnitResponse = await this._client
                .From<OptimisationProductionUnitPersistence>()
                .Insert(productionUnits);

            OptimisationProductionUnitPersistence? insertedProductionUnit = productionUnitResponse.Models.FirstOrDefault();

            if (insertedProductionUnit == null)
            {
                throw new DatabaseOperationException("Error in ResultRepository. Error creating result production unit.");
            }

            return (true, insertedProductionUnit);
        }
        catch (DatabaseOperationException e)
        {
            this._logger.LogError($"Error inserting optimisation result. {e.Message}");
            Console.WriteLine(e);

            return (false, null);
        }
    }
}