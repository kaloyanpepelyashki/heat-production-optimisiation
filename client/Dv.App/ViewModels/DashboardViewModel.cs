using System.Diagnostics;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace Dv.App.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dv.App.Models;
using Dv.App.Services;

using Dv.App.Interfaces;

public sealed partial class DashboardViewModel : ViewModelBase
{
    private readonly ILogger<DashboardViewModel> _logger;
    
    private readonly IApiService apiService;
    private string dashboardData = "Data will appear here once loaded...";
    private CancellationToken _ct =  new CancellationToken();

    public Task InitializationTask { get; private set; }
    private readonly Task _wakeUpServiceTask;

    [ObservableProperty] 
    private bool _isServiceWakingUp;
    [ObservableProperty] 
    private bool _serviceWokeUp;
    
    public DashboardViewModel(IApiService apiService, ILogger<DashboardViewModel> logger)
    { 
        _logger = logger;
        
        this.apiService = apiService;
        _wakeUpServiceTask = WakeUpService(_ct);
        this.InitializationTask = this.LoadDashboardDataAsync();
    }

    public string DashboardData
    {
        get => this.dashboardData;
        set => this.SetProperty(ref this.dashboardData, value);
    }

    private async Task WakeUpService(CancellationToken token)
    {
        _isServiceWakingUp = true;
        try
        {
            var response = await apiService.WakeUpService(BackendService.Sdm, token);

            if (!response)
            {
                _isServiceWakingUp = false;
                _serviceWokeUp = false;
            }
            
        }
        catch (OperationCanceledException)
        {
            _serviceWokeUp = false;
            _isServiceWakingUp = false;
            Debug.WriteLine("Error waking up service in DashboardViewModel. Process cancelled.");
            _logger.LogError("Error waking up service in DashboardViewModel. Process cancelled.");
        }
        catch (Exception e)
        {
            _isServiceWakingUp = false;
            _logger.LogError($"Error waking up service in DashboardViewModel: {e.Message}");
            Debug.WriteLine("Error waking up service in DashboardViewModel");
        }
        finally
        {
            _isServiceWakingUp = false;
            _serviceWokeUp = true;
            Debug.WriteLine("Woke up service in DashboardViewModel successfully");
            _logger.LogInformation("Woke up service in DashboardViewModel successfully");
        }
    }

    private async Task LoadDashboardDataAsync()
    {
        try
        {
            
            this.DashboardData = $"Pinging Render services...{Environment.NewLine}(Please wait, Render free tier can take up to 50s to wake up)";

            var sourceData = await this.apiService.GetAsync<List<SourceDataDto>>(BackendService.Sdm, "getAll");

            if (sourceData != null && sourceData.Any())
            {
                var first = sourceData.First();
                this.DashboardData = $"Success! SDM API responded. First Data entry: Heat Demand {first.HeatDemand}, Electricity Price {first.ElectricityPrice} from {first.TimeFrom}";
            }
            else
            {
                this.DashboardData = "Success! But SDM API returned an empty array.";
            }
        }
        catch (Exception ex)
        {
            this.DashboardData = $"Test Failed: {ex.Message} (Check if the Render URL is correct and the service is deployed)";
        }
    }
}
