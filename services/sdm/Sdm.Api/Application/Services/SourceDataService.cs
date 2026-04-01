using Sdm.Api.Application.Interfaces;
using Sdm.Api.Infrastructure.Persistence.PersistenceModels;

namespace Sdm.Api.Application.Services;

public class SourceDataService: ISourceDataService
{
    private readonly ISourceDataRepository _sourceDataRepository;

    public SourceDataService(ISourceDataRepository sourceDataRepository)
    {
        _sourceDataRepository = sourceDataRepository;
    }

    public async Task<List<SourceDataPersistence>> GetAllSourceData()
    {
        try
        {
            var sourceData = await _sourceDataRepository.GetAllSourceData();
            
            return sourceData;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in SourceDataService.GetSourceData: {e.Message}, {e.GetType()}");
            throw;
        }
    }
}