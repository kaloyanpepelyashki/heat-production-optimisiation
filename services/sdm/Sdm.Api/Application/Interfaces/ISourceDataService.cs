using Sdm.Api.Infrastructure.Persistence.PersistenceModels;

namespace Sdm.Api.Application.Interfaces;

public interface ISourceDataService
{
    Task<List<SourceDataPersistence>> GetAllSourceData();
}