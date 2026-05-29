namespace Opt.Api.Application.Interfaces;

using Opt.Api.Domain.Models;

public interface ISourceDataProvider
{
    Task<IReadOnlyList<SourceDataPoint>> GetSourceDataAsync(CancellationToken cancellationToken);
}
