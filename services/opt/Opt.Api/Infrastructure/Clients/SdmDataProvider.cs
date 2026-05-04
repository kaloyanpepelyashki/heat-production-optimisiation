using Opt.Api.Application.Exceptions;
using Opt.Api.Application.Interfaces;
using Opt.Api.Domain.Models;
using Opt.Api.DTOs;
using Opt.Api.Infrastructure.Options;

namespace Opt.Api.Infrastructure.Clients;

public sealed class SdmDataProvider : ISourceDataProvider
{
    private readonly HttpClient _httpClient;
    private readonly ExternalApiOptions _options;

    public SdmDataProvider(HttpClient httpClient, Microsoft.Extensions.Options.IOptions<ExternalApiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<IReadOnlyList<SourceDataPoint>> GetSourceDataAsync(CancellationToken cancellationToken)
    {
        try
        {
            var sourceData = await HttpRetryHelper.GetWithRetryAsync<List<SdmSourceDataResponseDto>>(
                _httpClient,
                _options.Sdm.SourceDataEndpoint,
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