using Opt.Api.Domain.Models;

namespace Opt.Api.Application.Interfaces;

public interface ISourceDataProvider
{
	Task<IReadOnlyList<SourceDataPoint>> GetSourceDataAsync(CancellationToken cancellationToken);
	Task PingAsync(CancellationToken cancellationToken);
}
