namespace Opt.Api.Infrastructure.Clients;

using Microsoft.Extensions.Caching.Memory;
using Opt.Api.Application.Exceptions;
using Opt.Api.Application.Interfaces;
using Opt.Api.Domain.Models;
using Opt.Api.Infrastructure.DTOs;
using Opt.Api.Infrastructure.Options;

public sealed class SdmDataProvider : ISourceDataProvider
{
    private const string CacheKey = "sdm-source-data";
    private static readonly SemaphoreSlim FetchLock = new(1, 1);
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(2);

    private readonly HttpClient _httpClient;
    private readonly ExternalApiOptions _options;
    private readonly IMemoryCache _cache;

    public SdmDataProvider(
        HttpClient httpClient,
        Microsoft.Extensions.Options.IOptions<ExternalApiOptions> options,
        IMemoryCache cache)
    {
        this._httpClient = httpClient;
        this._options = options.Value;
        this._cache = cache;
    }

    public async Task<IReadOnlyList<SourceDataPoint>> GetSourceDataAsync(CancellationToken cancellationToken)
    {
        if (this._cache.TryGetValue(CacheKey, out IReadOnlyList<SourceDataPoint>? cached))
        {
            return cached!;
        }

        await FetchLock.WaitAsync(cancellationToken);
        try
        {
            if (this._cache.TryGetValue(CacheKey, out cached))
            {
                return cached!;
            }

            var result = await this.FetchAsync(cancellationToken);
            this._cache.Set(CacheKey, result, CacheTtl);
            return result;
        }
        finally
        {
            FetchLock.Release();
        }
    }

    private async Task<IReadOnlyList<SourceDataPoint>> FetchAsync(CancellationToken cancellationToken)
    {
        try
        {
            var sourceData = await HttpRetryHelper.GetWithRetryAsync<List<SdmSourceDataResponseDto>>(
                this._httpClient,
                this._options.Sdm.SourceDataEndpoint,
                cancellationToken) ?? [];

            return sourceData.Select(x => new SourceDataPoint
            {
                Id = x.Id,
                PeriodId = x.PeriodId,
                TimeFrom = x.TimeFrom,
                TimeTo = x.TimeTo,
                HeatDemand = x.HeatDemand,
                ElectricityPrice = x.ElectricityPrice,
            }).ToList();
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or NotSupportedException)
        {
            throw new ExternalDataFetchException("Failed to fetch SDM data.", ex);
        }
    }
}