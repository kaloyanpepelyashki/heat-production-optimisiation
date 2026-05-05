using Opt.Api.Domain.Models;

namespace Opt.Api.Application.Interfaces;

public interface IAssetDataProvider
{
	Task<AssetDataBundle> GetAssetDataAsync(int maintenanceId, CancellationToken cancellationToken);
	Task PingAsync(CancellationToken cancellationToken);
}
