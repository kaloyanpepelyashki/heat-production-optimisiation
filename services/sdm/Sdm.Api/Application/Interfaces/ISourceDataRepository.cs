namespace Sdm.Api.Application.Interfaces;

using Sdm.Api.Infrastructure.Persistence.PersistenceModels;

public interface ISourceDataRepository
{
    Task<List<SourceDataPersistence>> GetAllSourceData();
}