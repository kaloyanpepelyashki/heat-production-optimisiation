using Sdm.Api.Infrastructure.Persistence.PersistenceModels;

namespace Sdm.Api.Application.Interfaces;

public interface ISourceDataRepository
{
    Task<List<SourceDataPersistence>> GetAllSourceData();
}