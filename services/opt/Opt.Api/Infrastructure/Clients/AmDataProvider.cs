namespace Opt.Api.Infrastructure.Clients;

using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using Opt.Api.Application.Exceptions;
using Opt.Api.Application.Interfaces;
using Opt.Api.Domain.Models;
using Opt.Api.Infrastructure.DTOs;
using Opt.Api.Infrastructure.Options;

public sealed class AmDataProvider : IAssetDataProvider
{
    private static readonly SemaphoreSlim FetchLock = new(1, 1);
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

    private readonly HttpClient _httpClient;
    private readonly ExternalApiOptions _options;
    private readonly IMemoryCache _cache;

    public AmDataProvider(
        HttpClient httpClient,
        Microsoft.Extensions.Options.IOptions<ExternalApiOptions> options,
        IMemoryCache cache)
    {
        this._httpClient = httpClient;
        this._options = options.Value;
        this._cache = cache;
    }

    public async Task<AssetDataBundle> GetAssetDataAsync(int maintenanceId, CancellationToken cancellationToken)
    {
        var cacheKey = $"am-assets-{maintenanceId}";

        if (this._cache.TryGetValue(cacheKey, out AssetDataBundle? cached))
        {
            return cached!;
        }

        await FetchLock.WaitAsync(cancellationToken);
        try
        {
            if (this._cache.TryGetValue(cacheKey, out cached))
            {
                return cached!;
            }

            var result = await this.FetchAllAsync(maintenanceId, cancellationToken);
            this._cache.Set(cacheKey, result, CacheTtl);
            return result;
        }
        finally
        {
            FetchLock.Release();
        }
    }

    private async Task<AssetDataBundle> FetchAllAsync(int maintenanceId, CancellationToken cancellationToken)
    {
        try
        {
            var gasBoilers = await this.GetWithRetryAsync<List<AmGasBoilerResponseDto>>(
                this._options.Am.GasBoilersEndpoint,
                cancellationToken);

            var oilBoilers = await this.GetWithRetryAsync<List<AmOilBoilerResponseDto>>(
                this._options.Am.OilBoilersEndpoint,
                cancellationToken);

            var electricBoilers = await this.GetWithRetryAsync<List<AmElectricBoilerResponseDto>>(
                this._options.Am.ElectricBoilersEndpoint,
                cancellationToken);

            var gasMotors = await this.GetWithRetryAsync<List<AmGasMotorResponseDto>>(
                this._options.Am.GasMotorsEndpoint,
                cancellationToken);

            var schedule = await this.GetWithRetryAsync<AmMaintenanceScheduleResponseDto>(
                this._options.Am.ResolveMaintenanceSchedulesEndpoint(maintenanceId),
                cancellationToken);

            var gasBoilersMapped = (gasBoilers ?? []).Select(x => new GasBoiler
            {
                Id = x.Id,
                Name = x.Name,
                MaxHeat = x.MaxHeat,
                ProductionCost = x.ProductionCost,
                Co2Emissions = x.Co2Emissions,
                GasConsumption = x.GasConsumption,
            }).ToList();

            var oilBoilersMapped = (oilBoilers ?? []).Select(x => new OilBoiler
            {
                Id = x.Id,
                Name = x.Name,
                MaxHeat = x.MaxHeat,
                ProductionCost = x.ProductionCost,
                Co2Emissions = x.Co2Emissions,
                OilConsumption = x.OilConsumption,
            }).ToList();

            var electricBoilersMapped = (electricBoilers ?? []).Select(x => new ElectricBoiler
            {
                Id = x.Id,
                Name = x.Name,
                MaxHeat = x.MaxHeat,
                ProductionCost = x.ProductionCost,
                MaxElectricity = x.MaxElectricity,
            }).ToList();

            var gasMotorsMapped = (gasMotors ?? []).Select(x => new GasMotor
            {
                Id = x.Id,
                Name = x.Name,
                MaxHeat = x.MaxHeat,
                MaxElectricity = x.MaxElectricity,
                ProductionCost = x.ProductionCost,
                Co2Emissions = x.Co2Emissions,
                GasConsumption = x.GasConsumption,
            }).ToList();

            var maintenanceSchedule = schedule is not null
                ? new MaintenanceSchedule
            {
                Id = schedule.Id,
                UnitType = schedule.UnitType,
                UnitId = schedule.UnitId,
                CreatedAt = schedule.CreatedAt,
                FromDate = schedule.FromDate,
                ToDate = schedule.ToDate,
                PeriodId = schedule.PeriodId,
                ScenarioId = schedule.ScenarioId,
            }
                : null;

            return new AssetDataBundle
            {
                GasBoilers = gasBoilersMapped,
                OilBoilers = oilBoilersMapped,
                ElectricBoilers = electricBoilersMapped,
                GasMotors = gasMotorsMapped,
                MaintenanceSchedule = maintenanceSchedule,
            };
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or NotSupportedException)
        {
            throw new ExternalDataFetchException("Failed to fetch AM data.", ex);
        }
    }

    private Task<T?> GetWithRetryAsync<T>(string url, CancellationToken cancellationToken) =>
        HttpRetryHelper.GetWithRetryAsync<T>(this._httpClient, url, cancellationToken);
}
