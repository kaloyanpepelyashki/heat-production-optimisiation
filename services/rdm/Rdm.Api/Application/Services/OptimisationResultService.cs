using Rdm.Api.Application.Exceptions;
using Rdm.Api.Application.Interfaces;
using Rdm.Api.Application.Model;
using Rdm.Api.Application.Services.Helpers;
using Rdm.Api.Inrastructure.Persistence;
using Rdm.Api.Inrastructure.Persistence.PersistenceModels;

namespace Rdm.Api.Application.Services;

/// <summary>
/// Provides all methods needed for working with the result object
/// </summary>
public class OptimisationResultService : IOptimisationResultService
{
    private IResultRepository _resultRepository;
    private ILogger<OptimisationResultService> _logger;

    public OptimisationResultService(IResultRepository resultRepository, ILogger<OptimisationResultService> logger)
    {
        _resultRepository = resultRepository;
        _logger = logger;
    }
    
    /// <summary>
    /// Gets all the optimisation results present in the database and the production unit objects associated with them.
    /// Calls the GetAllOptimisationResults method from the ResultRepository class.
    /// Maps the persistence models coming from database to abase domain model. Nests the models in the OptimisationRun returned. Countains information about the hourly optimisation and the production units
    /// used for each hour.
    /// </summary>
    /// <returns>A list of all optimisation results present</returns>
    public async Task<List<OptimisationRun>> GetAllOptimisationResults()
    {
        try
        {
            // Maps the persistence models coming from database to abase domain model
            List<OptimisationRunWithHourlyResultsPersistence> persistenceModel = await _resultRepository.GetAllOptimisationResults();
            List<OptimisationRun> optimisationResultsModels = persistenceModel.Select(obj => new OptimisationRun
            {
                Id = obj.Id,
                TimeFrom = obj.TimeFrom,
                TimeTo = obj.TimeTo,
                Scenario = obj.Scenario,
                PeriodType = obj.Type,

                OptimisationResultsHourly = obj.OptimisationResultsHourly.Select(hourly => new OptimisationResultsHourly
                {
                    Id = hourly.Id,
                    HeatProduction = hourly.HeatProduction,
                    ElectricityConsumption = hourly.ElectricityConsumption,
                    Co2Emissions = hourly.Co2Emissions,
                    Expenses = hourly.Expenses,
                    TimeFrom = hourly.TimeFrom,
                    TimeTo = hourly.TimeTo,

                    ProductionUnits = hourly.ProductionUnits.Select(opu => new ProductionUnit
                    {
                        Id = opu.Id,
                        ProductionUnitId = opu.ProductionUnitId,
                        ProductionUnitType = opu.ProductionUnitType,
                        HeatProduction = opu.HeatProduction,
                        ElectricityConsumption = opu.ElectricityConsumption,
                        Co2Emissions = opu.Co2Emissions,
                        Expenses = opu.Expenses,
                        Capacity = opu.Capacity
                    }).ToList()
                }).ToList()
            }).ToList();

            return optimisationResultsModels;
        }
        catch (DatabaseOperationException e)
        {
            _logger.LogError(
                $"Error in OptimisationResultService. Database operation error. Failed to get all optimisation results due to a database born error : {e.Message}, {e.GetType()}");
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError($"Error in OptimisationResultService. Failed to get all optimisations Error: {e.Message}, {e.GetType()}");
            throw;
        }
    }

    
    /// <summary>
    /// In charge of saving a new optimisation run object to the database.
    /// Utilises the Result repository to handle the database entry creation
    /// </summary>
    /// <param name="optimisationRun"></param>
    /// <returns></returns>
    public async Task<bool> SaveOptimisationRun(OptimisationRun optimisationRun)
    {
        try
        {
            OptimisationRunPersistenceWrapper optimisationRunPersistence =
                OptimisationModelsMapper.ToPersistenceWrapper(optimisationRun);
            var creationResult = await _resultRepository.SaveOptimisationResult(optimisationRunPersistence);

            return creationResult;
        }
        catch (ArgumentException e)
        {
            _logger.LogError($"Error translating domain to persistence model: {e.Message} ");
            throw e;
        }
        catch (DatabaseOperationException e)
        {
            _logger.LogError($"Error in OptimisationResultService. Database operation error. Failed to create a new optimisation run due to a database born error: {e.Message}, {e.GetType()}");
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError($"Error in OptimisationResultService. Failed to get all optimisations Error: {e.Message}, {e.GetType()}");
            throw;
        }
    }
} 