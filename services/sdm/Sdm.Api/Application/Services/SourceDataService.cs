namespace Sdm.Api.Application.Services;

using Sdm.Api.Application.Interfaces;
using Sdm.Api.Infrastructure.Persistence.PersistenceModels;

public class SourceDataService : ISourceDataService
{
    private readonly ISourceDataRepository _sourceDataRepository;

    public SourceDataService(ISourceDataRepository sourceDataRepository)
    {
        this._sourceDataRepository = sourceDataRepository;
    }

    public async Task<List<SourceDataPersistence>> GetAllSourceData()
    {
        try
        {
            var sourceData = await this._sourceDataRepository.GetAllSourceData();

            return sourceData;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in SourceDataService.GetSourceData: {e.Message}, {e.GetType()}");
            throw;
        }
    }
}