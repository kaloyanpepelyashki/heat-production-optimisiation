namespace Opt.Api.Application.Interfaces;

using Opt.Api.Domain.Models;

public interface IAssetDataProvider
{
    Task<AssetDataBundle> GetAssetDataAsync(int maintenanceId, CancellationToken cancellationToken);
}
