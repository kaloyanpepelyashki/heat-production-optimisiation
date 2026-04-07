using Opt.Api.Application.Interfaces;
using Opt.Api.Infrastructure.Persistence.PersistenceModels;

namespace Opt.Api.Application.Services;

public class SelectedProductionUnitsService: ISelectedProductionUnitsService
{
    private readonly ISelectedProductionUnitsRepository _selectedProductionUnitstRepository;

    public SelectedProductionUnitsService(ISelectedProductionUnitsRepository selectedProductionUnitstRepository)
    {
        _selectedProductionUnitstRepository = selectedProductionUnitstRepository;
    }

    public async Task<List<SelectedProductionUnitsPersistence>> GetAllSelectedProductionUnitsAsync()
    {
        try
        {
            var selectedProductionUnitst = await _selectedProductionUnitstRepository.GetAllSelectedProductionUnitsAsync();
            return selectedProductionUnitst;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in NetPrductionCostService.GetNetPrductionCost: {e.Message}, {e.GetType()}");
            throw;
        }
    }
}