namespace Sdm.Api.Application.Interfaces;

using Sdm.Api.Infrastructure.Persistence.PersistenceModels;

public interface ISourceDataService
{
    Task<List<SourceDataPersistence>> GetAllSourceData();
}