using Opt.Api.Domain.Models;

namespace Opt.Api.Application.Interfaces;

public interface IAssetDataProvider
{
	Task<AssetDataBundle> GetAssetDataAsync(CancellationToken cancellationToken);
}
