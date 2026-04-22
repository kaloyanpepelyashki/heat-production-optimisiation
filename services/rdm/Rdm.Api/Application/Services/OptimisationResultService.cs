using Rdm.Api.Application.Exceptions;
using Rdm.Api.Application.Interfaces;
using Rdm.Api.Application.Model;
using Rdm.Api.Inrastructure.Persistence;
using Rdm.Api.Inrastructure.Persistence.PersistenceModels;

namespace Rdm.Api.Application.Services;

public class OptimisationResultService
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
    /// </summary>
    /// <returns>A list of all optimisation results present</returns>
    public async Task<List<OptimisationResult>> GetAllOptimisationResults()
    {
        try
        {
            List<ResultPersistence> persistenceModel = await _resultRepository.GetAllOptimisationResults();
            List<OptimisationResult> optimisationResultsModels = persistenceModel.Select(obj =>
            {
                List<ProductionUnit> productionUnits = obj.optimisationProductionUnits.Select(opu => new ProductionUnit
                {
                    Id = opu.Id,
                    ProductionUnitId = opu.ProductionUnitId,
                    ProductionUnitType = opu.ProductionUnitType
                }).ToList();

                return new OptimisationResult
                {
                    Id = obj.Id,
                    HeatProduction = obj.HeatProduction,
                    ElectricityConsumption = obj.ElectricityConsumption,
                    Expenses = obj.Expenses,
                    Profit = obj.Profit,
                    ProducedCo2Emissions = obj.ProducedCo2Emissions,
                    DateRun = obj.DateRun,
                    ProductionUnits = productionUnits,
                };
            }).ToList();

            return optimisationResultsModels;
        }
        catch (DatabaseOperationException e)
        {
            _logger.LogError($"Error in OptimisationResultService. Failed to get all optimisation results due to database born error : {e.Message}, {e.GetType()}");
            throw; 
        }
    }
} 