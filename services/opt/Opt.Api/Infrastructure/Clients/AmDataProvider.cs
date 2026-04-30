using System.Net.Http.Json;
using Opt.Api.Application.Exceptions;
using Opt.Api.Application.Interfaces;
using Opt.Api.Domain.Models;
using Opt.Api.DTOs;
using Opt.Api.Infrastructure.Options;

namespace Opt.Api.Infrastructure.Clients;

public sealed class AmDataProvider : IAssetDataProvider
{
    private readonly HttpClient _httpClient;
    private readonly ExternalApiOptions _options;

    public AmDataProvider(HttpClient httpClient, Microsoft.Extensions.Options.IOptions<ExternalApiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<AssetDataBundle> GetAssetDataAsync(int maintenanceId, CancellationToken cancellationToken)
    {
        try
        {
            var gasBoilersTask = _httpClient.GetFromJsonAsync<List<AmGasBoilerResponseDto>>(
                _options.Am.GasBoilersEndpoint,
                cancellationToken);

            var oilBoilersTask = _httpClient.GetFromJsonAsync<List<AmOilBoilerResponseDto>>(
                _options.Am.OilBoilersEndpoint,
                cancellationToken);

            var electricBoilersTask = _httpClient.GetFromJsonAsync<List<AmElectricBoilerResponseDto>>(
                _options.Am.ElectricBoilersEndpoint,
                cancellationToken);

            var gasMotorsTask = _httpClient.GetFromJsonAsync<List<AmGasMotorResponseDto>>(
                _options.Am.GasMotorsEndpoint,
                cancellationToken);
           
            var maintenanceScheduleTask = _httpClient.GetFromJsonAsync<AmMaintenanceScheduleResponseDto>(
                _options.Am.ResolveMaintenanceSchedulesEndpoint(maintenanceId),
                cancellationToken);
            
            await Task.WhenAll(gasBoilersTask, oilBoilersTask, electricBoilersTask, gasMotorsTask, maintenanceScheduleTask); 

            var gasBoilers = (await gasBoilersTask ?? []).Select(x => new GasBoiler
            {
                Id = x.Id,
                Name = x.Name,
                MaxHeat = x.MaxHeat,
                ProductionCost = x.ProductionCost,
                Co2Emissions = x.Co2Emissions,
                GasConsumption = x.GasConsumption,
            }).ToList();

            var oilBoilers = (await oilBoilersTask ?? []).Select(x => new OilBoiler
            {
                Id = x.Id,
                Name = x.Name,
                MaxHeat = x.MaxHeat,
                ProductionCost = x.ProductionCost,
                Co2Emissions = x.Co2Emissions,
                OilConsumption = x.OilConsumption,
            }).ToList();

            var electricBoilers = (await electricBoilersTask ?? []).Select(x => new ElectricBoiler
            {
                Id = x.Id,
                Name = x.Name,
                MaxHeat = x.MaxHeat,
                ProductionCost = x.ProductionCost,
                MaxElectricity = x.MaxElectricity,
            }).ToList();

            var gasMotors = (await gasMotorsTask ?? []).Select(x => new GasMotor
            {
                Id = x.Id,
                Name = x.Name,
                MaxHeat = x.MaxHeat,
                MaxElectricity = x.MaxElectricity,
                ProductionCost = x.ProductionCost,
                Co2Emissions = x.Co2Emissions,
                GasConsumption = x.GasConsumption,
            }).ToList();

            var maintenanceSchedule = (await maintenanceScheduleTask) is { } schedule
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
                GasBoilers = gasBoilers,
                OilBoilers = oilBoilers,
                ElectricBoilers = electricBoilers,
                GasMotors = gasMotors,
                MaintenanceSchedule = maintenanceSchedule,
            };
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or NotSupportedException)
        {
            throw new ExternalDataFetchException("Failed to fetch AM data.", ex);
        }
    }
}