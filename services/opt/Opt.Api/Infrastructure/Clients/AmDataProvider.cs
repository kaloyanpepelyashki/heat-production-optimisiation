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
            var gasBoilers = await _httpClient.GetFromJsonAsync<List<AmGasBoilerResponseDto>>(
                _options.Am.GasBoilersEndpoint,
                cancellationToken);

            await Task.Delay(1500, cancellationToken); // Rate limiting delay

            var oilBoilers = await _httpClient.GetFromJsonAsync<List<AmOilBoilerResponseDto>>(
                _options.Am.OilBoilersEndpoint,
                cancellationToken);

            await Task.Delay(1500, cancellationToken); // Rate limiting delay

            var electricBoilers = await _httpClient.GetFromJsonAsync<List<AmElectricBoilerResponseDto>>(
                _options.Am.ElectricBoilersEndpoint,
                cancellationToken);

            await Task.Delay(1500, cancellationToken); // Rate limiting delay

            var gasMotors = await _httpClient.GetFromJsonAsync<List<AmGasMotorResponseDto>>(
                _options.Am.GasMotorsEndpoint,
                cancellationToken);

            await Task.Delay(1500, cancellationToken); // Rate limiting delay
           
            var schedule = await _httpClient.GetFromJsonAsync<AmMaintenanceScheduleResponseDto>(
                _options.Am.ResolveMaintenanceSchedulesEndpoint(maintenanceId),
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
}